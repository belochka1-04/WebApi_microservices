using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Xunit;
using Service = WebApi_GoogleSearchTemplatesService.Services.DatabaseService;

namespace ModelChecker.Lease.Tests;

public sealed class GoogleSerpBatchTests
{
    private sealed class FailOnce(bool afterCommit) : DbTransactionInterceptor
    {
        public bool Failed;
        public override Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            if (afterCommit && !Failed) { Failed = true; throw new TimeoutException("Simulated lost commit response"); }
            return Task.CompletedTask;
        }
    }

    private sealed class FailAfterSaveOnce : SaveChangesInterceptor
    {
        public bool Failed;
        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = default)
        {
            if (!Failed) { Failed = true; throw new TimeoutException("Simulated failure before commit"); }
            return ValueTask.FromResult(result);
        }
    }

    [GoogleLeaseSqlFact]
    public async Task RetryingBatchHandlesRollbackAndLostCommitResponseWithoutDuplicates()
    {
        var connection = Environment.GetEnvironmentVariable("GOOGLE_LEASE_TEST_CONNECTION")!;
        foreach (var afterCommit in new[] { false, true })
        {
            await using var sql = new SqlConnection(connection);
            await sql.OpenAsync();
            await using var seed = sql.CreateCommand();
            seed.CommandText = "INSERT dbo.GoogleModelRequests([Request],CreatedAt,Status,AttemptCount) OUTPUT inserted.Id VALUES(@word,SYSUTCDATETIME(),1,0)";
            seed.Parameters.AddWithValue("@word", "QA-SERP-" + Guid.NewGuid().ToString("N"));
            var id = Convert.ToInt32(await seed.ExecuteScalarAsync());
            try
            {
                var commitFault = new FailOnce(afterCommit);
                var saveFault = new FailAfterSaveOnce();
                IInterceptor interceptor = afterCommit ? commitFault : saveFault;
                await using var db = new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>()
                    .UseSqlServer(connection, o => o.EnableRetryOnFailure(2, TimeSpan.Zero, null))
                    .AddInterceptors(interceptor).Options);
                var service = new Service(db);
                GoogleSerpRaw Row(string suffix) => new() { GoogleModelRequestId=id, QueryText="QA query",
                    GooglePage=0, Title=suffix, Url="https://example.invalid/" + suffix };
                var ids = await service.InsertGoogleSerpRawBatchAsync(new[] { Row("one"), Row("two") });
                Assert.True(afterCommit ? commitFault.Failed : saveFault.Failed);
                Assert.Equal(2, ids.Count);
                Assert.Equal(2, await db.GoogleSerpRaws.CountAsync(r => r.GoogleModelRequestId == id));
                await service.InsertGoogleSerpRawBatchAsync(new[] { Row("replacement") });
                var stored = await db.GoogleSerpRaws.Where(r => r.GoogleModelRequestId == id).ToListAsync();
                Assert.Equal("replacement", Assert.Single(stored).Title);
            }
            finally
            {
                await using var cleanup = sql.CreateCommand();
                cleanup.CommandText="DELETE dbo.google_serp_raw WHERE google_model_request_id=@id; DELETE dbo.GoogleModelRequests WHERE Id=@id";
                cleanup.Parameters.AddWithValue("@id", id);
                await cleanup.ExecuteNonQueryAsync();
            }
        }
    }
}
