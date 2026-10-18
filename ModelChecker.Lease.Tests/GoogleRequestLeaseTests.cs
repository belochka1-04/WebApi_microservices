using KameraData.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApi_GoogleSearchTemplatesService.Services;
using Xunit;
using Service = WebApi_GoogleSearchTemplatesService.Services.DatabaseService;

namespace ModelChecker.Lease.Tests;

public sealed class GoogleLeaseSqlFactAttribute : FactAttribute
{
    public GoogleLeaseSqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOOGLE_LEASE_TEST_CONNECTION")))
            Skip = "Requires an explicitly configured test database.";
    }
}

public sealed class GoogleRequestLeaseTests
{
    private sealed class Fixture : IDisposable
    {
        public readonly string Connection = Environment.GetEnvironmentVariable("GOOGLE_LEASE_TEST_CONNECTION")!;
        public readonly string Word = "QA-CONTINUOUS-GOOGLE-" + Guid.NewGuid().ToString("N");
        public readonly string Owner = "qa-google:" + Guid.NewGuid().ToString("N");
        public readonly int Id;
        public Fixture() => Id = Convert.ToInt32(Sql("""
INSERT dbo.GoogleModelRequests([Request],CreatedAt,Status,WorkerId,LeaseUntil,AttemptCount)
OUTPUT inserted.Id VALUES(@word,SYSUTCDATETIME(),2,@owner,DATEADD(MINUTE,10,SYSUTCDATETIME()),1);
""", ("@word", Word), ("@owner", Owner)));
        public object? Sql(string sql, params (string Name, object Value)[] parameters)
        {
            using var c = new SqlConnection(Connection); c.Open();
            using var command = new SqlCommand(sql, c) { CommandTimeout = 10 };
            foreach (var p in parameters) command.Parameters.AddWithValue(p.Name, p.Value);
            return command.ExecuteScalar();
        }
        public KameraDbContext Context() => new(new DbContextOptionsBuilder<KameraDbContext>().UseSqlServer(Connection).Options);
        public GoogleRequestLeaseRequest Request() => new() { WorkerId = Owner, AttemptCount = 1, LeaseSeconds = 120,
            RetryDelaySeconds = 30, MaxAttempts = 3, Error = "QA failure" };
        public void Dispose() => Sql("DELETE dbo.GoogleModelRequests WHERE Id=@id AND [Request]=@word", ("@id", Id), ("@word", Word));
    }

    [GoogleLeaseSqlFact]
    public async Task OnlyCurrentOwnerAndAttemptCanRenewAndComplete_AndCompletionReceiptIsIdempotent()
    {
        using var f = new Fixture(); await using var db = f.Context(); var service = new Service(db);
        var wrong = f.Request(); wrong.WorkerId = "old-owner";
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, wrong, "renew"));
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, wrong, "complete"));
        wrong = f.Request(); wrong.AttemptCount = 2;
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, wrong, "complete"));
        Assert.True(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "renew"));
        Assert.True(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "complete"));
        Assert.True(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "complete"));
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "fail"));
        Assert.Equal((byte)1, f.Sql("SELECT Status FROM dbo.GoogleModelRequests WHERE Id=@id", ("@id", f.Id)));
    }

    [GoogleLeaseSqlFact]
    public async Task ExpiredLeaseCannotRenewCompleteOrFail()
    {
        using var f = new Fixture(); await using var db = f.Context();
        // Keep the test row locked throughout expiry checks so live workers cannot claim it.
        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.GoogleModelRequests SET LeaseUntil=DATEADD(SECOND,-1,SYSUTCDATETIME()) WHERE Id={f.Id}");
        var service = new Service(db);
        foreach (var operation in new[] { "renew", "complete", "fail" })
            Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), operation));
        await transaction.RollbackAsync();
    }

    [GoogleLeaseSqlFact]
    public async Task FailureDefersRetryWithoutAllowingOldOwnerToWrite()
    {
        using var f = new Fixture(); await using var db = f.Context(); var service = new Service(db);
        Assert.True(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "fail"));
        Assert.Equal(1, f.Sql("SELECT CASE WHEN Status=2 AND WorkerId IS NULL AND LeaseUntil>SYSUTCDATETIME() AND LastError='QA failure' THEN 1 ELSE 0 END FROM dbo.GoogleModelRequests WHERE Id=@id", ("@id", f.Id)));
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "renew"));
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, f.Request(), "complete"));
    }

    [GoogleLeaseSqlFact]
    public async Task FinalFailureClosesRequestAndStaleAttemptCannotCloseNewOwner()
    {
        using var f = new Fixture(); await using var db = f.Context(); var service = new Service(db);
        f.Sql("UPDATE dbo.GoogleModelRequests SET AttemptCount=3 WHERE Id=@id", ("@id", f.Id));
        var final = f.Request(); final.AttemptCount = 3;
        Assert.True(await service.UpdateGoogleRequestLeaseAsync(f.Id, final, "fail"));
        Assert.Equal((byte)3, f.Sql("SELECT Status FROM dbo.GoogleModelRequests WHERE Id=@id", ("@id", f.Id)));
        f.Sql("UPDATE dbo.GoogleModelRequests SET Status=2,WorkerId='new-owner',AttemptCount=4,LeaseUntil=DATEADD(MINUTE,10,SYSUTCDATETIME()) WHERE Id=@id", ("@id", f.Id));
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, final, "complete"));
        Assert.False(await service.UpdateGoogleRequestLeaseAsync(f.Id, final, "fail"));
    }
}
