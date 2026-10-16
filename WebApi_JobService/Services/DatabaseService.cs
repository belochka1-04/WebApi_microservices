using KameraData.Data;
using KameraData.Data.Dtos;
using KameraData.Data.Models;
using KameraData.Data.Other;
using KameraData.Events;
using MassTransit;
using MassTransit.Extensions.Hosting;
using MassTransit.Transports;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_JobService.Services;

namespace WebApi_JobService.Services
{
    public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
    {
        private readonly KameraDbContext _dbContext;
        private readonly NLog.ILogger _logger;
        private readonly UserServiceClient _userServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;  // ← ПОЛЕ (не локальная переменная)


        public DatabaseService(KameraDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<DatabaseService> logger)// ← DI автоматически                                                                               
        {
            _dbContext = dbContext;
            //_userServiceClient = userServiceClient;
            _publishEndpoint = publishEndpoint;  // ← ПРИСВАИВАЕМ ПОЛЕ
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<bool> IsDatabaseHealthyAsync()
        {
            try
            {
                // 🔥 +1: не только connect, но и query
                var canConnect = await _dbContext.Database.CanConnectAsync();
                if (!canConnect) return false;

                // 🔥 +2: тестовая query
                var count = await _dbContext.JobDescriptionAndNotes
                    .FromSqlRaw("SELECT COUNT(*) FROM JobDescriptionAndNotes WITH (NOLOCK)")
                    .AsNoTracking()
                    .CountAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "База данных недоступна");
                return false;
            }
        }


        #region job_description_and_notes
        public async Task<IEnumerable<JobDescriptionAndNote>> GetJobsAsync()
        {
            try
            {
                return await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.Status == "0" && j.GoogleConfirmed == 1)
                    .AsNoTracking()  // 🔥 +1: read-only, быстрее
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при получении заданий"); // 🔥 +2: structured logging
                return Array.Empty<JobDescriptionAndNote>(); // 🔥 +3: вместо new List
            }
        }

        public async Task<IEnumerable<JobDescriptionAndNote>> ClaimJobsAsync(string workerId, int batchSize, int leaseSeconds)
        {
            if (string.IsNullOrWhiteSpace(workerId))
            {
                throw new ArgumentException("WorkerId is required.", nameof(workerId));
            }

            batchSize = Math.Clamp(batchSize, 1, 100);
            leaseSeconds = Math.Clamp(leaseSeconds, 60, 21600);

            const string sql = @"
;WITH jobs_to_claim AS
(
    SELECT TOP (@BatchSize) *
    FROM dbo.job_description_and_notes WITH (UPDLOCK, READPAST, ROWLOCK)
    WHERE google_confirmed = 1
      AND (status = '0'
       OR (status = '2' AND lease_until_utc IS NOT NULL AND lease_until_utc < SYSUTCDATETIME()))
    ORDER BY id
)
UPDATE jobs_to_claim
SET status = '2',
    worker_id = @WorkerId,
    lease_until_utc = DATEADD(SECOND, @LeaseSeconds, SYSUTCDATETIME()),
    attempt_count = ISNULL(attempt_count, 0) + 1,
    processing_started_at_utc = COALESCE(processing_started_at_utc, SYSUTCDATETIME()),
    processing_completed_at_utc = NULL,
    processing_last_error = NULL
OUTPUT
    inserted.id,
    inserted.job_id,
    inserted.job_description,
    inserted.job_notes,
    inserted.pic_counter,
    inserted.pic_recon,
    inserted.status,
    inserted.brand,
    inserted.model,
    inserted.serial_number,
    inserted.ocr_text,
    inserted.is_rating_plate_percentage,
    inserted.completion_cost,
    inserted.image_tokens,
    inserted.sticker_link,
    inserted.original_brand,
    inserted.google_confirmed,
    inserted.full_text,
    inserted.worker_id,
    inserted.lease_until_utc,
    inserted.attempt_count,
    inserted.processing_started_at_utc,
    inserted.processing_completed_at_utc,
    inserted.processing_last_error;";

            var result = new List<JobDescriptionAndNote>();

            try
            {
                var connection = _dbContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 120;

                AddParameter(command, "@WorkerId", workerId);
                AddParameter(command, "@BatchSize", batchSize);
                AddParameter(command, "@LeaseSeconds", leaseSeconds);

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(MapClaimedJobDescription(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при захвате заданий job_description_and_notes");
            }

            foreach (var job in result)
            {
                await TryInsertJobProcessingEventAsync(new JobProcessingEventRequest
                {
                    JobId = job.JobId,
                    EntityType = "job_description_and_notes",
                    EntityId = job.Id,
                    Component = "jobservice",
                    Stage = "job_description_claimed",
                    Status = "success",
                    Message = "Job description claimed for processing.",
                    WorkerId = workerId,
                    PodName = Environment.GetEnvironmentVariable("HOSTNAME"),
                    DetailsJson = $"{{\"batchSize\":{batchSize},\"leaseSeconds\":{leaseSeconds},\"attemptCount\":{job.AttemptCount}}}"
                });
            }

            return result;
        }

        private static void AddParameter(System.Data.Common.DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }


        private async Task TryInsertJobProcessingEventAsync(JobProcessingEventRequest request)
        {
            try
            {
                await InsertJobProcessingEventAsync(request);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка записи события обработки JobId={JobId}, Component={Component}, Stage={Stage}",
                    request.JobId, request.Component, request.Stage);
            }
        }

        private async Task TryInsertBusinessEventAsync(BusinessEventRequest request)
        {
            try
            {
                await InsertBusinessEventAsync(request);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка записи бизнес-события EventName={EventName}, JobId={JobId}, UserId={UserId}",
                    request.EventName, request.JobId, request.UserId);
            }
        }

        public async Task InsertJobProcessingEventAsync(JobProcessingEventRequest request)
        {
            const string sql = @"
INSERT INTO dbo.job_processing_events
(
    correlation_id,
    job_id,
    entity_type,
    entity_id,
    component,
    stage,
    status,
    message,
    model_number,
    part_number,
    worker_id,
    pod_name,
    duration_ms,
    error_message,
    details_json
)
VALUES
(
    @CorrelationId,
    @JobId,
    @EntityType,
    @EntityId,
    @Component,
    @Stage,
    @Status,
    @Message,
    @ModelNumber,
    @PartNumber,
    @WorkerId,
    @PodName,
    @DurationMs,
    @ErrorMessage,
    @DetailsJson
);";

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 30;

            AddParameter(command, "@CorrelationId", request.CorrelationId);
            AddParameter(command, "@JobId", request.JobId);
            AddParameter(command, "@EntityType", TrimTo(request.EntityType, 64));
            AddParameter(command, "@EntityId", request.EntityId);
            AddParameter(command, "@Component", TrimTo(request.Component, 128));
            AddParameter(command, "@Stage", TrimTo(request.Stage, 128));
            AddParameter(command, "@Status", TrimTo(request.Status, 64));
            AddParameter(command, "@Message", TrimTo(request.Message, 512));
            AddParameter(command, "@ModelNumber", TrimTo(request.ModelNumber, 100));
            AddParameter(command, "@PartNumber", TrimTo(request.PartNumber, 100));
            AddParameter(command, "@WorkerId", TrimTo(request.WorkerId, 128));
            AddParameter(command, "@PodName", TrimTo(request.PodName, 128));
            AddParameter(command, "@DurationMs", request.DurationMs);
            AddParameter(command, "@ErrorMessage", request.ErrorMessage);
            AddParameter(command, "@DetailsJson", request.DetailsJson);

            await command.ExecuteNonQueryAsync();

            if (string.Equals(request.Stage, "model_number_saved", StringComparison.OrdinalIgnoreCase))
            {
                await TryInsertBusinessEventAsync(new BusinessEventRequest
                {
                    JobId = request.JobId,
                    EventName = "model_number_saved",
                    EventCategory = "job_result",
                    Source = request.Component,
                    Feature = "model_number_checker",
                    Status = request.Status,
                    DurationMs = request.DurationMs,
                    MetadataJson = request.DetailsJson
                });
            }
            else if (string.Equals(request.Status, "error", StringComparison.OrdinalIgnoreCase))
            {
                await TryInsertBusinessEventAsync(new BusinessEventRequest
                {
                    JobId = request.JobId,
                    EventName = "job_processing_error",
                    EventCategory = "job_result",
                    Source = request.Component,
                    Feature = request.Stage,
                    Status = "error",
                    DurationMs = request.DurationMs,
                    MetadataJson = request.ErrorMessage == null
                        ? request.DetailsJson
                        : $"{{\"error\":\"{EscapeJson(request.ErrorMessage)}\"}}"
                });
            }
        }

        public async Task<IEnumerable<JobProcessingEventRequest>> GetJobProcessingEventsAsync(int jobId)
        {
            const string sql = @"
SELECT
    correlation_id,
    job_id,
    entity_type,
    entity_id,
    component,
    stage,
    status,
    message,
    model_number,
    part_number,
    worker_id,
    pod_name,
    duration_ms,
    error_message,
    details_json,
    created_at_utc
FROM dbo.job_processing_events WITH (NOLOCK)
WHERE job_id = @JobId
ORDER BY created_at_utc, id;";

            var events = new List<JobProcessingEventRequest>();
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 30;
            AddParameter(command, "@JobId", jobId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                events.Add(new JobProcessingEventRequest
                {
                    CorrelationId = GetNullableField<Guid>(reader, "correlation_id"),
                    JobId = GetNullableField<int>(reader, "job_id"),
                    EntityType = GetNullableField<string>(reader, "entity_type"),
                    EntityId = GetNullableField<int>(reader, "entity_id"),
                    Component = GetNullableField<string>(reader, "component") ?? string.Empty,
                    Stage = GetNullableField<string>(reader, "stage") ?? string.Empty,
                    Status = GetNullableField<string>(reader, "status") ?? string.Empty,
                    Message = GetNullableField<string>(reader, "message"),
                    ModelNumber = GetNullableField<string>(reader, "model_number"),
                    PartNumber = GetNullableField<string>(reader, "part_number"),
                    WorkerId = GetNullableField<string>(reader, "worker_id"),
                    PodName = GetNullableField<string>(reader, "pod_name"),
                    DurationMs = GetNullableField<int>(reader, "duration_ms"),
                    ErrorMessage = GetNullableField<string>(reader, "error_message"),
                    DetailsJson = GetNullableField<string>(reader, "details_json"),
                    CreatedAtUtc = GetNullableField<DateTime>(reader, "created_at_utc")
                });
            }

            return events;
        }

        public async Task InsertBusinessEventAsync(BusinessEventRequest request)
        {
            const string sql = @"
INSERT INTO dbo.business_events
(
    event_id,
    session_id,
    user_id,
    telegram_user_id,
    job_id,
    event_name,
    event_category,
    source,
    feature,
    status,
    numeric_value,
    duration_ms,
    metadata_json,
    occurred_at_utc
)
VALUES
(
    COALESCE(@EventId, NEWID()),
    @SessionId,
    @UserId,
    @TelegramUserId,
    @JobId,
    @EventName,
    @EventCategory,
    @Source,
    @Feature,
    @Status,
    @NumericValue,
    @DurationMs,
    @MetadataJson,
    COALESCE(@OccurredAtUtc, SYSUTCDATETIME())
);";

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 30;

            AddParameter(command, "@EventId", request.EventId);
            AddParameter(command, "@SessionId", request.SessionId);
            AddParameter(command, "@UserId", request.UserId);
            AddParameter(command, "@TelegramUserId", request.TelegramUserId);
            AddParameter(command, "@JobId", request.JobId);
            AddParameter(command, "@EventName", TrimTo(request.EventName, 128));
            AddParameter(command, "@EventCategory", TrimTo(request.EventCategory, 64));
            AddParameter(command, "@Source", TrimTo(request.Source, 64));
            AddParameter(command, "@Feature", TrimTo(request.Feature, 128));
            AddParameter(command, "@Status", TrimTo(request.Status, 64));
            AddParameter(command, "@NumericValue", request.NumericValue);
            AddParameter(command, "@DurationMs", request.DurationMs);
            AddParameter(command, "@MetadataJson", request.MetadataJson);
            AddParameter(command, "@OccurredAtUtc", request.OccurredAtUtc);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<BusinessEventRequest>> GetBusinessEventsForJobAsync(int jobId)
        {
            const string sql = @"
SELECT
    event_id,
    session_id,
    user_id,
    telegram_user_id,
    job_id,
    event_name,
    event_category,
    source,
    feature,
    status,
    numeric_value,
    duration_ms,
    metadata_json,
    occurred_at_utc,
    created_at_utc
FROM dbo.business_events WITH (NOLOCK)
WHERE job_id = @JobId
ORDER BY occurred_at_utc, id;";

            return await ReadBusinessEventsAsync(sql, ("@JobId", jobId));
        }

        public async Task<IEnumerable<BusinessEventRequest>> GetBusinessEventsForUserAsync(int userId, int take)
        {
            const string sql = @"
SELECT TOP (@Take)
    event_id,
    session_id,
    user_id,
    telegram_user_id,
    job_id,
    event_name,
    event_category,
    source,
    feature,
    status,
    numeric_value,
    duration_ms,
    metadata_json,
    occurred_at_utc,
    created_at_utc
FROM dbo.business_events WITH (NOLOCK)
WHERE user_id = @UserId
ORDER BY occurred_at_utc DESC, id DESC;";

            take = Math.Clamp(take, 1, 1000);
            return await ReadBusinessEventsAsync(sql, ("@UserId", userId), ("@Take", take));
        }

        private async Task<IEnumerable<BusinessEventRequest>> ReadBusinessEventsAsync(
            string sql,
            params (string Name, object Value)[] parameters)
        {
            var events = new List<BusinessEventRequest>();
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 30;
            foreach (var parameter in parameters)
            {
                AddParameter(command, parameter.Name, parameter.Value);
            }

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                events.Add(new BusinessEventRequest
                {
                    EventId = GetNullableField<Guid>(reader, "event_id"),
                    SessionId = GetNullableField<Guid>(reader, "session_id"),
                    UserId = GetNullableField<int>(reader, "user_id"),
                    TelegramUserId = GetNullableField<long>(reader, "telegram_user_id"),
                    JobId = GetNullableField<int>(reader, "job_id"),
                    EventName = GetNullableField<string>(reader, "event_name") ?? string.Empty,
                    EventCategory = GetNullableField<string>(reader, "event_category") ?? string.Empty,
                    Source = GetNullableField<string>(reader, "source"),
                    Feature = GetNullableField<string>(reader, "feature"),
                    Status = GetNullableField<string>(reader, "status"),
                    NumericValue = GetNullableField<decimal>(reader, "numeric_value"),
                    DurationMs = GetNullableField<int>(reader, "duration_ms"),
                    MetadataJson = GetNullableField<string>(reader, "metadata_json"),
                    OccurredAtUtc = GetNullableField<DateTime>(reader, "occurred_at_utc"),
                    CreatedAtUtc = GetNullableField<DateTime>(reader, "created_at_utc")
                });
            }

            return events;
        }

        private static string? TrimTo(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        private static JobDescriptionAndNote MapClaimedJobDescription(System.Data.Common.DbDataReader reader)
        {
            return new JobDescriptionAndNote
            {
                Id = GetField<int>(reader, "id"),
                JobId = GetField<int>(reader, "job_id"),
                JobDescription = GetNullableField<string>(reader, "job_description"),
                JobNotes = GetNullableField<string>(reader, "job_notes"),
                PicCounter = GetNullableField<int>(reader, "pic_counter"),
                PicRecon = GetNullableField<int>(reader, "pic_recon"),
                Status = GetNullableField<string>(reader, "status"),
                Brand = GetNullableField<string>(reader, "brand"),
                Model = GetNullableField<string>(reader, "model"),
                SerialNumber = GetNullableField<string>(reader, "serial_number"),
                OcrText = GetNullableField<string>(reader, "ocr_text"),
                IsRatingPlatePercentage = GetNullableField<int>(reader, "is_rating_plate_percentage"),
                CompletionCost = GetNullableField<decimal>(reader, "completion_cost"),
                ImageTokens = GetNullableField<int>(reader, "image_tokens"),
                StickerLink = GetNullableField<string>(reader, "sticker_link"),
                OriginalBrand = GetNullableField<string>(reader, "original_brand"),
                GoogleConfirmed = GetNullableField<int>(reader, "google_confirmed"),
                FullText = GetNullableField<string>(reader, "full_text"),
                WorkerId = GetNullableField<string>(reader, "worker_id"),
                LeaseUntilUtc = GetNullableField<DateTime>(reader, "lease_until_utc"),
                AttemptCount = GetField<int>(reader, "attempt_count"),
                ProcessingStartedAtUtc = GetNullableField<DateTime>(reader, "processing_started_at_utc"),
                ProcessingCompletedAtUtc = GetNullableField<DateTime>(reader, "processing_completed_at_utc"),
                ProcessingLastError = GetNullableField<string>(reader, "processing_last_error")
            };
        }

        private static T GetField<T>(System.Data.Common.DbDataReader reader, string name)
        {
            return (T)reader.GetValue(reader.GetOrdinal(name));
        }

        private static T? GetNullableField<T>(System.Data.Common.DbDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? default : (T)reader.GetValue(ordinal);
        }

        public async Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID)
        {
            try
            {
                return await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobID)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения JobDescriptionAndNote JobId={JobId}", jobID);
                return Array.Empty<JobDescriptionAndNote>();
            }
        }

        public async Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status)
        {
            try
            {
                if (jobId <= 0)
                {
                    _logger.Warn($"Некорректный JobId={jobId}");
                    return;
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    var updatedRows = await _dbContext.JobDescriptionAndNotes
                        .Where(j => j.JobId == jobId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(j => j.Status, status.ToString()));

                    _logger.Info($"✅ Обновлено Status={status} для {updatedRows} записей JobId={jobId}");
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Ошибка обновления Status JobId={jobId}: {ex}");
                throw;
            }
        }

        public async Task UpdateDescriptionAndNotesDescriptionAsync(int jobId, string fulltext)
        {
            try
            {
                if (jobId <= 0 || string.IsNullOrWhiteSpace(fulltext))
                {
                    _logger.Warn($"Некорректные параметры: JobId={jobId}, fulltext length={fulltext?.Length ?? 0}");
                    return;
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    // Заменяем ' на пробел для SQL safety
                    var safeFulltext = fulltext.Replace('\'', ' ');

                    var updatedRows = await _dbContext.JobDescriptionAndNotes
                        .Where(j => j.JobId == jobId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(j => j.JobDescription, safeFulltext));

                    _logger.Info($"✅ Обновлено JobDescription для {updatedRows} записей JobId={jobId}, length={safeFulltext.Length}");
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Ошибка обновления JobDescription JobId={jobId}: {ex}");
                throw;
            }
        }

        public async Task UpdateDescriptionAndNotesPicCountAsync(int jobId, int pic_count)
        {
            try
            {
                if (jobId <= 0)
                {
                    _logger.Warn($"Некорректный JobId={jobId}");
                    return;
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    var updatedRows = await _dbContext.JobDescriptionAndNotes
                        .Where(j => j.JobId == jobId)
                        .ExecuteUpdateAsync(s => s.SetProperty(j => j.PicRecon, pic_count));

                    _logger.Info($"✅ Обновлено PicRecon={pic_count} для {updatedRows} записей JobId={jobId}");
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Ошибка обновления PicRecon JobId={jobId}: {ex}");
                throw;
            }
        }

        public async Task InsertDescriptionAndNotesDescriptionAsync(int jobId, string fulltext)
        {
            try
            {
                if (jobId <= 0 || string.IsNullOrWhiteSpace(fulltext))
                {
                    _logger.Warn($"Некорректные параметры: JobId={jobId}, fulltext length={fulltext?.Length ?? 0}");
                    return;
                }

                var jobDescription = new JobDescriptionAndNote
                {
                    JobId = jobId,
                    JobDescription = fulltext.Replace('\'', ' '), // Escaping
                    Status = "0"
                };

                const int maxRetries = 3;
                var delayMs = 50; // Меньше для INSERT

                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        await _dbContext.JobDescriptionAndNotes.AddAsync(jobDescription);
                        await _dbContext.SaveChangesAsync();

                        _logger.Info($"✅ Добавлена JobDescriptionAndNote для JobId={jobId}, ID={jobDescription.Id}");
                        return;
                    }
                    catch (DbUpdateException ex) when (IsDeadlock(ex) && retry < maxRetries - 1)
                    {
                        _dbContext.Entry(jobDescription).State = EntityState.Detached; // Очистка change tracker
                        _logger.Warn($"⚠️ Deadlock INSERT #{retry + 1}/{maxRetries} для JobId={jobId}. Retry через {delayMs}ms");
                        await Task.Delay(delayMs);
                        delayMs *= 2; // 50, 100, 200ms
                    }
                }

                _logger.Error($"❌ INSERT failed после {maxRetries} попыток для JobId={jobId}");
                throw new InvalidOperationException($"Не удалось вставить JobDescriptionAndNote для JobId={jobId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"💥 Критическая ошибка INSERT JobDescriptionAndNote JobId={jobId}: {ex}");
                throw;
            }
        }



        public async Task MarkJobDescriptionsGoogleConfirmedAsync(string requestWord)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestWord)) return;
                var normalized = requestWord.Trim().ToUpper();

                const int maxRetries = 3;
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        var tableName = _dbContext.Model.FindEntityType(typeof(JobDescriptionAndNote))!.GetTableName()!;
                        var sql = $@"UPDATE [job_description_and_notes] WITH (ROWLOCK, UPDLOCK)
                            SET [google_confirmed] = 1           -- 🔥 snake_case!
                            WHERE ([google_confirmed] IS NULL OR [google_confirmed] = 0)
                            AND UPPER(LTRIM(RTRIM(ISNULL([job_description],'') + ISNULL([job_notes], '')))) = @p0";


                        var updatedRows = await _dbContext.Database
                            .ExecuteSqlRawAsync(sql, normalized);

                        _logger.Info($"✅ Raw SQL: обновлено {updatedRows} записей GoogleConfirmed для '{requestWord}'");
                        return;
                    }
                    catch (SqlException ex) when (IsTransientError(ex) && retry < maxRetries - 1)
                    {
                        _logger.Warn($"⚠️ SQL Error #{retry + 1}: {ex.Number} - {ex.Message}");
                        await Task.Delay(100 * (retry + 1));
                    }
                }
                throw new InvalidOperationException($"Failed after 3 retries for '{requestWord}'");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"💥 MarkJobDescriptionsGoogleConfirmedAsync failed: {requestWord}");
                throw;
            }
        }

        private static bool IsTransientError(SqlException ex) =>
            ex.Number is 1205 or 1222 or 49918 or 49919; // Deadlock, timeout, transient


        private static bool IsDeadlock(DbUpdateException ex) =>
         ex.InnerException is SqlException sqlEx &&
         (sqlEx.Number == 1205 /* deadlock */ || sqlEx.Number == 1222 /* lock timeout */);

        #endregion

        #region jobs
        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            try
            {
                var job = await _dbContext.Jobs.Include(x => x.User)
                    .Where(j => j.Id == jobId)
                    .Select(j => new Job
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        JobNumber = j.JobNumber,
                        JobLink = j.JobLink,
                        Token = j.Token,
                        UpdateRequestStatus = j.UpdateRequestStatus,
                        User = j.User,
                    })
                    .FirstOrDefaultAsync();

                if (job == null)
                {
                    throw new Exception("Job not found.");
                }

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при получении заданий: " + ex);
                return new Job(); // Возвращаем новый экземпляр Job в случае ошибки
            }
        }

        public async Task<JobDto?> GetJobDtoByIdAsync(int jobId)
        {
            try
            {
                var job = await _dbContext.Jobs
                    .Include(x => x.User)
                    .Where(j => j.Id == jobId)
                    .Select(j => new JobDto
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        TelegramId = j.TelegramId,
                        JobNumber = j.JobNumber,
                        JobLink = j.JobLink,
                        Token = j.Token,
                        UpdateRequestStatus = j.UpdateRequestStatus,
                        Status = j.Status
                    })
                    .FirstOrDefaultAsync();

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при получении Job");
                return null;
            }
        }
        public async Task<IEnumerable<Job>> GetJobByLink(string jobLink)
        {
            try
            {
                var jobs = await _dbContext.Jobs
                    .Where(j => j.JobLink == jobLink)
                    .Select(j => new Job
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        JobNumber = j.JobNumber
                    })
                    .ToListAsync();

                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб линк:" + jobLink + " - " + ex);
                return new List<Job>();
            }
        }

        
        public async Task<Job> AddJobByTgAsync(int telegramId)
        {
            var maxJobNumber = await _dbContext.Jobs
                .MaxAsync(j => Convert.ToInt32(j.JobNumber ?? "0"));  // ← Защита от null

            var newJob = new Job
            {
                TelegramId = telegramId,
                JobNumber = (maxJobNumber + 1).ToString(),
                Status = "Pending"
            };

            await _dbContext.Jobs.AddAsync(newJob);
            await _dbContext.SaveChangesAsync();
            
            // добавили _publishEndpoint
            await _publishEndpoint.Publish(new JobCreatedEvent
            {
                JobId = newJob.Id,
                TelegramId = telegramId
            });

            await TryInsertBusinessEventAsync(new BusinessEventRequest
            {
                TelegramUserId = telegramId,
                JobId = newJob.Id,
                EventName = "job_created",
                EventCategory = "job",
                Source = "telegram",
                Feature = "telegram_job",
                Status = "success",
                MetadataJson = "{\"entryPoint\":\"addJobByTg\"}"
            });

            return newJob;
        }

        public async Task<int> GetTodaysOperationsCountAsync(int userId)
        {
            try
            {
                var today = DateTime.Today;

                return await _dbContext.Jobs
                    .Where(j => j.UserId == userId && j.CreatedAt >= today)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при подсчете операций за сегодня для пользователя {userId}", userId);
                return 0;
            }
        }

        public async Task<Job> CreateJobFromTextAsync(int userId, string? content, string? brand)
        {
            try
            {
                var job = new Job
                {
                    UserId = userId,
                    JobNumber = null,
                    JobLink = null,
                    Token = null,
                    UpdateRequestStatus = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Jobs.AddAsync(job);

                var note = new JobDescriptionAndNote
                {
                    Job = job,
                    JobId = job.Id,        // EF сам подставит после SaveChanges, но можно оставить явно
                    JobDescription = content,
                    OriginalBrand = brand,
                    Status = "0"
                };

                await _dbContext.JobDescriptionAndNotes.AddAsync(note);
                await _dbContext.SaveChangesAsync();

                await TryInsertBusinessEventAsync(new BusinessEventRequest
                {
                    UserId = userId,
                    JobId = job.Id,
                    EventName = "job_created",
                    EventCategory = "job",
                    Source = "api",
                    Feature = "text_request",
                    Status = "success",
                    MetadataJson = $"{{\"contentLength\":{(content?.Length ?? 0)},\"hasBrand\":{ToJsonBool(!string.IsNullOrWhiteSpace(brand))}}}"
                });

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при создании задания из текста для пользователя {userId}", userId);
                throw;
            }
        }

        public async Task<Job> CreateJobFromImageAsync(CreateJobFromImageDto dto)
        {
            try
            {
                var job = new Job
                {
                    UserId = dto.UserId,
                    JobNumber = null,
                    JobLink = null,
                    Token = null,
                    UpdateRequestStatus = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Jobs.AddAsync(job);

                var note = new JobDescriptionAndNote
                {
                    Job = job,
                    JobId = job.Id,
                    OriginalBrand = dto.Brand,
                    Model = dto.ModelNumber,
                    JobDescription = dto.ModelNumber,
                    StickerLink = dto.ImageUrl,
                    SerialNumber = dto.SerialNumber,
                    Status = "0"
                };

                await _dbContext.JobDescriptionAndNotes.AddAsync(note);

                // если позже решишь хранить распознавания, тут можно добавлять связанные сущности

                await _dbContext.SaveChangesAsync();

                await TryInsertBusinessEventAsync(new BusinessEventRequest
                {
                    UserId = dto.UserId,
                    JobId = job.Id,
                    EventName = "job_created",
                    EventCategory = "job",
                    Source = "api",
                    Feature = "image_request",
                    Status = "success",
                    MetadataJson = $"{{\"hasImage\":{ToJsonBool(!string.IsNullOrWhiteSpace(dto.ImageUrl))},\"hasBrand\":{ToJsonBool(!string.IsNullOrWhiteSpace(dto.Brand))},\"hasModel\":{ToJsonBool(!string.IsNullOrWhiteSpace(dto.ModelNumber))},\"hasSerial\":{ToJsonBool(!string.IsNullOrWhiteSpace(dto.SerialNumber))}}}"
                });

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при создании задания из изображения для пользователя {userId}", dto.UserId);
                throw;
            }
        }

        public async Task<Job?> GetFullJobByIdAsync(int jobId)
        {
            try
            {
                return await _dbContext.Jobs
                    .Include(j => j.JobDescriptionAndNote)
                    .Include(j => j.JobDocs)
                    .FirstOrDefaultAsync(j => j.Id == jobId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при получении полного Job {jobId}", jobId);
                return null;
            }
        }


        #endregion

        private static string ToJsonBool(bool value)
        {
            return value ? "true" : "false";
        }

        private static string EscapeJson(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

    }
}
