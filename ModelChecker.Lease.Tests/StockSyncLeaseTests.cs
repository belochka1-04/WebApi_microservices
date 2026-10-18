using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApi_StockCredsService.Services;
using Xunit;
using Service = WebApi_StockCredsService.Services.DatabaseService;

namespace ModelChecker.Lease.Tests;

public sealed class StockSqlFactAttribute : FactAttribute
{
    public StockSqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("STOCK_SYNC_TEST_CONNECTION")))
            Skip = "Requires an explicitly configured test database.";
    }
}

public sealed class StockSyncLeaseTests
{
    private sealed class Fixture : IDisposable
    {
        public readonly string Connection = Environment.GetEnvironmentVariable("STOCK_SYNC_TEST_CONNECTION")!;
        public readonly string Owner = "qa-stock-sync:" + Guid.NewGuid().ToString("N");
        public readonly Guid Lock = Guid.NewGuid();
        public readonly int Id;
        public Fixture()
        {
            Id = Convert.ToInt32(Sql("""
INSERT dbo.stock_creds(user_id,stock_name,contact_types_id,sync_switch,sync_freq,doc_types_id)
OUTPUT inserted.id
SELECT TOP (1) 1036,@name,contact_types_id,'off','99999',3 FROM dbo.stock_creds WHERE user_id=1036;
""", ("@name", "QA-STOCK-SYNC-" + Guid.NewGuid().ToString("N")[..12])));
            if (Id <= 0) throw new InvalidOperationException("No test-user stock template available.");
            Sql("UPDATE dbo.stock_creds SET sync_lock_id=@lock,sync_locked_by=@owner,sync_lease_until=DATEADD(MINUTE,5,SYSUTCDATETIME()) WHERE id=@id",
                ("@lock", Lock), ("@owner", Owner), ("@id", Id));
        }
        public object? Sql(string sql, params (string Name, object Value)[] parameters)
        {
            using var connection = new SqlConnection(Connection); connection.Open();
            using var command = new SqlCommand(sql, connection) { CommandTimeout = 10 };
            foreach (var p in parameters) command.Parameters.AddWithValue(p.Name, p.Value);
            return command.ExecuteScalar();
        }
        public KameraDbContext Context() => new(new DbContextOptionsBuilder<KameraDbContext>().UseSqlServer(Connection).Options);
        public StockSyncLeaseRequest Request(bool success = true) => new()
        { WorkerId = Owner, LockId = Lock, LeaseSeconds = 120, RetryDelaySeconds = 30, Success = success, Error = "QA failure" };
        public ActionExecutingContext Action(string? owner = null, Guid? token = null, int? bodyStock = null)
        {
            var http = new DefaultHttpContext(); http.Request.Method = "PUT";
            http.Request.Headers["X-StockSync-Stock"] = Id.ToString();
            http.Request.Headers["X-StockSync-Lock"] = (token ?? Lock).ToString();
            http.Request.Headers["X-StockSync-Worker"] = owner ?? Owner;
            return new(new ActionContext(http, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>(),
                bodyStock.HasValue ? new Dictionary<string, object?> { ["request"] = new UserStock { StockId = bodyStock.Value } }
                    : new Dictionary<string, object?>(), new object());
        }
        public void Dispose() => Sql("DELETE dbo.stock_creds WHERE id=@id AND user_id=1036 AND stock_name LIKE 'QA-STOCK-SYNC-%'", ("@id", Id));
    }

    [StockSqlFact]
    public async Task RenewalAndFinishRequireCurrentOwnerLockAndUnexpiredLease()
    {
        using var f = new Fixture(); await using var db = f.Context(); var service = new Service(db);
        var wrong = f.Request(); wrong.LockId = Guid.NewGuid();
        Assert.False(await service.UpdateSyncLease(f.Id, wrong, false));
        Assert.False(await service.UpdateSyncLease(f.Id, wrong, true));
        wrong = f.Request(); wrong.WorkerId = "other";
        Assert.False(await service.UpdateSyncLease(f.Id, wrong, false));
        Assert.True(await service.UpdateSyncLease(f.Id, f.Request(), false));
        Assert.True(await service.UpdateSyncLease(f.Id, f.Request(false), true));
        Assert.Equal("QA failure", f.Sql("SELECT sync_last_error FROM dbo.stock_creds WHERE id=@id", ("@id", f.Id)));
        Assert.Equal(1, f.Sql("SELECT CASE WHEN sync_lease_until>SYSUTCDATETIME() AND sync_lock_id IS NULL THEN 1 ELSE 0 END FROM dbo.stock_creds WHERE id=@id", ("@id", f.Id)));
        Assert.False(await service.UpdateSyncLease(f.Id, f.Request(), true));
        f.Sql("UPDATE dbo.stock_creds SET sync_lock_id=@lock,sync_locked_by=@owner,sync_lease_until=DATEADD(SECOND,-1,SYSUTCDATETIME()) WHERE id=@id",
            ("@lock", f.Lock), ("@owner", f.Owner), ("@id", f.Id));
        Assert.False(await service.UpdateSyncLease(f.Id, f.Request(), false));
    }

    [StockSqlFact]
    public async Task GuardedWriteCommitsOnlyForCurrentOwnerAndRollsBackFailedAction()
    {
        using var f = new Fixture();
        async Task Execute(string? owner, bool fail)
        {
            await using var db = f.Context(); var action = f.Action(owner); var called = false;
            await new StockSyncLeaseFilter(db).OnActionExecutionAsync(action, async () =>
            {
                called = true;
                await new Service(db).UpdateSyncState(f.Id, new StockSyncStateRequest { RecordsCount = fail ? 99 : 7, SyncSwitch = "off" });
                return new ActionExecutedContext(action, new List<IFilterMetadata>(), new object())
                { Result = fail ? new BadRequestResult() : new NoContentResult() };
            });
            Assert.Equal(owner == null, called);
            if (owner != null) Assert.IsType<ConflictObjectResult>(action.Result);
        }
        await Execute(null, false);
        Assert.Equal(7, f.Sql("SELECT records_count FROM dbo.stock_creds WHERE id=@id", ("@id", f.Id)));
        await Execute(null, true);
        await Execute("stale-owner", false);
        Assert.Equal(7, f.Sql("SELECT records_count FROM dbo.stock_creds WHERE id=@id", ("@id", f.Id)));
    }

    [StockSqlFact]
    public async Task GuardTransactionEnclosesExistingUserStockInsertAndRollsItBack()
    {
        using var f = new Fixture(); await using var db = f.Context(); var action = f.Action();
        await new StockSyncLeaseFilter(db).OnActionExecutionAsync(action, async () =>
        {
            var users = new WebApi_UserService.Services.DatabaseService(db,
                Microsoft.Extensions.Logging.Abstractions.NullLogger<WebApi_UserService.Services.DatabaseService>.Instance);
            await users.InsertUserStockAsync(new UserStock { StockId = f.Id, UserId = 1036, PartNumber = "QA-SYNC", Qty = 1 });
            Assert.Equal(1, await db.UserStocks.CountAsync(row => row.StockId == f.Id));
            return new ActionExecutedContext(action, new List<IFilterMetadata>(), new object()) { Result = new BadRequestResult() };
        });
        Assert.Equal(0, f.Sql("SELECT COUNT(*) FROM dbo.user_stocks WHERE stock_id=@id", ("@id", f.Id)));
    }

    [StockSqlFact]
    public async Task SqlFailureDuringGuardedUserStockInsertIsPropagated()
    {
        using var f = new Fixture(); await using var db = f.Context(); var action = f.Action();
        await Assert.ThrowsAsync<SqlException>(() => new StockSyncLeaseFilter(db).OnActionExecutionAsync(action, async () =>
        {
            var users = new WebApi_UserService.Services.DatabaseService(db,
                Microsoft.Extensions.Logging.Abstractions.NullLogger<WebApi_UserService.Services.DatabaseService>.Instance);
            await users.InsertUserStockAsync(new UserStock { StockId = f.Id, UserId = 1036, PartNumber = new string('X', 1000) });
            return new ActionExecutedContext(action, new List<IFilterMetadata>(), new object()) { Result = new NoContentResult() };
        }));
        Assert.Equal(0, f.Sql("SELECT COUNT(*) FROM dbo.user_stocks WHERE stock_id=@id", ("@id", f.Id)));
    }

    [StockSqlFact]
    public async Task GuardRejectsWritesToAnotherStockAndLeavesLeaseIntact()
    {
        using var f = new Fixture(); await using var db = f.Context();
        var action = f.Action(bodyStock: f.Id + 1);
        await new StockSyncLeaseFilter(db).OnActionExecutionAsync(action, () => throw new Exception("Should not execute"));
        Assert.IsType<BadRequestObjectResult>(action.Result);
        Assert.True(await new Service(db).UpdateSyncLease(f.Id, f.Request(), true));
        Assert.Equal(DBNull.Value, f.Sql("SELECT sync_lock_id FROM dbo.stock_creds WHERE id=@id", ("@id", f.Id)));
    }
}
