using System.Data;
using System.Text.Json;
using KameraData.Data;
using KameraData.Data.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebApi_ModelCatalogService.Services;

public sealed class ModelBatchUpsertService
{
    public const int MaxModelsPerBatch = 500;

    private readonly KameraDbContext _dbContext;
    private readonly ILogger<ModelBatchUpsertService> _logger;

    public ModelBatchUpsertService(
        KameraDbContext dbContext, ILogger<ModelBatchUpsertService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ModelBatchUpsertResult> UpsertAsync(
        ModelBatchUpsertRequest request, CancellationToken cancellationToken)
    {
        if (request.Models.Count == 0)
            return new ModelBatchUpsertResult();

        if (request.Models.Count > MaxModelsPerBatch)
            throw new ArgumentException($"A batch may contain at most {MaxModelsPerBatch} models.");

        // One transaction per bounded chunk. A repeated chunk sees its previous inserts/links
        // and therefore makes no further changes if the response was lost after COMMIT.
        var payload = JsonSerializer.Serialize(request.Models, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await ExecuteOnceAsync(
                    payload, request.GoogleModelRequestId, request.ChunkIndex, cancellationToken);
            }
            catch (SqlException ex) when (attempt < 3 && (ex.Number == 1205 || ex.Number == 1222 || ex.Number == 51000))
            {
                _logger.LogWarning(ex,
                    "Model batch SQL contention for RequestId={RequestId}, attempt={Attempt}, models={Count}",
                    request.GoogleModelRequestId, attempt, request.Models.Count);
                await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt), cancellationToken);
            }
        }
    }

    private async Task<ModelBatchUpsertResult> ExecuteOnceAsync(
        string payload, int requestId, int chunkIndex, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_dbContext.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(BatchSql, connection)
        {
            CommandTimeout = 120
        };
        command.Parameters.Add("@payload", SqlDbType.NVarChar, -1).Value = payload;
        command.Parameters.Add("@requestId", SqlDbType.Int).Value = requestId;
        command.Parameters.Add("@chunkIndex", SqlDbType.Int).Value = chunkIndex;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            throw new InvalidOperationException("Model batch did not return a result.");

        return new ModelBatchUpsertResult
        {
            Received = reader.GetInt32(0),
            Inserted = reader.GetInt32(1),
            Updated = reader.GetInt32(2),
            Unchanged = reader.GetInt32(3),
            MissingBrandModel = reader.GetInt32(4),
            Duplicates = reader.GetInt32(5)
        };
    }

    private const string BatchSql = """
        SET NOCOUNT ON;
        SET XACT_ABORT ON;
        SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
        BEGIN TRANSACTION;

        DECLARE @lockResult int;
        EXEC @lockResult = sys.sp_getapplock
            @Resource = N'ModelCatalog.ModelBatchUpsert',
            @LockMode = N'Exclusive',
            @LockOwner = N'Transaction',
            @LockTimeout = 30000;
        IF @lockResult < 0 THROW 51000, 'Could not acquire model batch lock.', 1;

        DECLARE @payloadHash binary(32) = HASHBYTES('SHA2_256', @payload);
        IF @requestId > 0 AND EXISTS (
            SELECT 1 FROM dbo.model_batch_receipt
            WHERE request_id = @requestId AND chunk_index = @chunkIndex
              AND payload_hash = @payloadHash
        )
        BEGIN
            COMMIT TRANSACTION;
            SELECT received, inserted, updated, unchanged, missing_brand_model, duplicates
            FROM dbo.model_batch_receipt
            WHERE request_id = @requestId AND chunk_index = @chunkIndex
              AND payload_hash = @payloadHash;
            RETURN;
        END;

        SELECT CONVERT(int, j.[key]) AS RowNo,
               c.SiteId, c.BrandTitle, c.CleanedModel, c.Title, c.Link
        INTO #input
        FROM OPENJSON(@payload) AS j
        CROSS APPLY OPENJSON(j.value) WITH (
            SiteId int '$.siteId',
            BrandTitle nvarchar(150) '$.brandTitle',
            CleanedModel nvarchar(255) '$.cleanedModel',
            Title nvarchar(150) '$.title',
            Link nvarchar(255) '$.link'
        ) AS c;

        DECLARE @received int = (SELECT COUNT(*) FROM #input);
        DECLARE @missing int;
        DECLARE @inserted int;
        DECLARE @updated int;
        DECLARE @unchanged int;
        DECLARE @duplicates int;

        -- BrandModel is an existing, global code lookup. SiteId is deliberately not
        -- used here; it is part of the Model key below. Resolve each code once.
        SELECT DISTINCT UPPER(LTRIM(RTRIM(BrandTitle))) AS BrandKey
        INTO #brandKeys
        FROM #input;

        SELECT b.BrandKey, matched.Id AS BrandModelId
        INTO #brands
        FROM #brandKeys AS b
        OUTER APPLY (
            SELECT TOP (1) bm.Id
            FROM dbo.BrandModel AS bm
            WHERE UPPER(LTRIM(RTRIM(bm.Code))) = b.BrandKey
            ORDER BY bm.Id
        ) AS matched;

        SELECT i.RowNo, i.SiteId, i.CleanedModel, i.Title, i.Link,
               bm.BrandModelId,
               ROW_NUMBER() OVER (
                   PARTITION BY i.SiteId, bm.BrandModelId, i.CleanedModel
                   ORDER BY i.RowNo DESC
               ) AS DuplicateRank
        INTO #resolved
        FROM #input AS i
        LEFT JOIN #brands AS bm
            ON bm.BrandKey = UPPER(LTRIM(RTRIM(i.BrandTitle)));

        SELECT @missing = COUNT(*) FROM #resolved WHERE BrandModelId IS NULL;
        SELECT @duplicates = COUNT(*) FROM #resolved
        WHERE BrandModelId IS NOT NULL AND DuplicateRank > 1;

        SELECT r.SiteId, r.BrandModelId, r.CleanedModel, r.Title, r.Link,
               m.Id AS ModelId
        INTO #matched
        FROM #resolved AS r
        OUTER APPLY (
            SELECT TOP (1) model.Id
            FROM dbo.Model AS model
            WHERE model.SiteId = r.SiteId
              AND model.BrandModelId = r.BrandModelId
              AND model.cleaned_model = r.CleanedModel
            ORDER BY model.Id
        ) AS m
        WHERE r.BrandModelId IS NOT NULL AND r.DuplicateRank = 1;

        CREATE TABLE #changed (ModelId int NOT NULL, OldLink nvarchar(255) NULL);

        UPDATE model
        SET model.Link = candidate.Link, model.LinkState = 1
        OUTPUT deleted.Id, deleted.Link INTO #changed (ModelId, OldLink)
        FROM dbo.Model AS model
        JOIN #matched AS candidate ON candidate.ModelId = model.Id
        WHERE ISNULL(model.Link, N'') <> candidate.Link;
        SET @updated = @@ROWCOUNT;

        INSERT INTO dbo.model_link_history (model_id, old_link, changed_at, source)
        SELECT ModelId, ISNULL(OldLink, N''), SYSUTCDATETIME(), N'google_xmlstock'
        FROM #changed;

        INSERT INTO dbo.Model
            (BrandModelId, cleaned_model, Link, LinkState, SiteId, Title)
        SELECT BrandModelId, CleanedModel, Link, 1, SiteId, Title
        FROM #matched
        WHERE ModelId IS NULL
          AND NOT EXISTS (
              SELECT 1 FROM dbo.Model AS existing WITH (UPDLOCK, HOLDLOCK)
              WHERE existing.SiteId = #matched.SiteId
                AND existing.BrandModelId = #matched.BrandModelId
                AND existing.cleaned_model = #matched.CleanedModel
          );
        SET @inserted = @@ROWCOUNT;

        SELECT @unchanged = COUNT(*) - @inserted - @updated FROM #matched;

        IF @requestId > 0
            INSERT INTO dbo.model_batch_receipt
                (request_id, chunk_index, payload_hash, received, inserted,
                 updated, unchanged, missing_brand_model, duplicates)
            VALUES
                (@requestId, @chunkIndex, @payloadHash, @received, @inserted,
                 @updated, @unchanged, @missing, @duplicates);

        COMMIT TRANSACTION;
        SELECT @received, @inserted, @updated, @unchanged, @missing, @duplicates;
        """;
}
