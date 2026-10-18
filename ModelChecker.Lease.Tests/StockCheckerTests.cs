using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApi_ModelNumberService.Services;
using Xunit;
using Service=WebApi_ModelNumberService.Services.DatabaseService;

namespace ModelChecker.Lease.Tests;

public sealed class StockSqlFactAttribute:FactAttribute
{
    public StockSqlFactAttribute() {if(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("STOCK_CHECKER_TEST_CONNECTION"))) Skip="Requires explicit test SQL connection.";}
}
public sealed class StockCheckerTests
{
    private sealed class Fixture:IAsyncDisposable
    {
        public readonly KameraDbContext Db=new(new DbContextOptionsBuilder<KameraDbContext>()
            .UseSqlServer(Environment.GetEnvironmentVariable("STOCK_CHECKER_TEST_CONNECTION")).Options);
        private IDbContextTransaction? transaction;
        public int JobId;
        public Service Service=>new(Db);
        public async Task Start(bool transactional=true)
        {
            if(transactional) transaction=await Db.Database.BeginTransactionAsync();
            var job=new Job{UserId=1036,JobNumber="QA-STOCK-POOL-"+Guid.NewGuid().ToString("N"),UpdateRequestStatus="9"};
            Db.Jobs.Add(job);await Db.SaveChangesAsync();JobId=job.Id;
        }
        public async Task<ModelNumber> Row(string confirmed="1",int attempt=0,int ready=1)
        {
            var row=new ModelNumber{JobId=JobId,ModelNumber1="QAMODEL123",MN_request="QA-STOCK-POOL",Confirmed="3",DocCounter=0,jdConfirmed=ready};
            Db.ModelNumbers.Add(row);await Db.SaveChangesAsync();Db.ChangeTracker.Clear();
            await Db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET confirmed={confirmed},attempt_count={attempt},jd_confirmed={ready},worker_id='owner',lease_until=DATEADD(minute,10,SYSUTCDATETIME()) WHERE id={row.Id}");
            return row;
        }
        public Task Expire(int id)=>Db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET lease_until=DATEADD(minute,-1,SYSUTCDATETIME()) WHERE id={id}");
        public StockCheckerLeaseRequest Token(ModelNumber row)=>new(){WorkerId=row.WorkerId!,AttemptCount=row.AttemptCount,Error="failure",MaxAttempts=3,RetryDelaySeconds=2};
        public async Task<ModelNumber> Read(int id)=>await Db.ModelNumbers.AsNoTracking().SingleAsync(x=>x.Id==id);
        public async ValueTask DisposeAsync()
        {
            try {
                if(transaction!=null){await transaction.RollbackAsync();await transaction.DisposeAsync();}
                else {
                    Db.Database.SetCommandTimeout(90);
                    await Db.Database.ExecuteSqlInterpolatedAsync($"DELETE dbo.model_numbers WHERE job_id={JobId}; DELETE dbo.jobs WHERE id={JobId} AND job_number LIKE 'QA-STOCK-POOL-%'");
                }
            }
            finally {await Db.DisposeAsync();}
        }
    }

    [StockSqlFact]
    public async Task ClaimRequiresReadyJob_SerializesModelsWithinJob_AndCanRefillAfterCompletion()
    {
        await using var f=new Fixture();await f.Start();var notReady=await f.Row(ready:0);var first=await f.Row();var second=await f.Row();
        // The real trigger finishes doc preparation for confirmed=1; use a two-row update
        // so its single-row path does not turn this deliberately unready fixture back on.
        await f.Db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET jd_confirmed=CASE WHEN id={notReady.Id} THEN 0 ELSE 1 END WHERE id IN ({notReady.Id},{first.Id})");
        var claim=await f.Service.ClaimStockCheckerAsync("owner-a",900,3,f.JobId);
        Assert.Equal(first.Id,claim!.Id);Assert.Equal(1,claim.AttemptCount);
        Assert.Null(await f.Service.ClaimStockCheckerAsync("owner-b",900,3,f.JobId));
        Assert.True(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,f.Token(claim),"complete"));
        Assert.True(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,f.Token(claim),"complete"));
        var next=await f.Service.ClaimStockCheckerAsync("owner-b",900,3,f.JobId);
        Assert.Equal(second.Id,next!.Id);Assert.Equal("1",(await f.Read(notReady.Id)).Confirmed);
    }

    [StockSqlFact]
    public async Task OwnerAttemptAndExpiryFenceAllLeaseOperations()
    {
        await using var f=new Fixture();await f.Start();await f.Row();var claim=(await f.Service.ClaimStockCheckerAsync("owner-a",900,3,f.JobId))!;
        foreach(var op in new[]{"renew","complete","fail","reject"}) {
            var wrong=f.Token(claim);wrong.WorkerId="wrong";Assert.False(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,wrong,op));
            wrong=f.Token(claim);wrong.AttemptCount++;Assert.False(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,wrong,op));
        }
        Assert.True(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,f.Token(claim),"renew"));
        await f.Expire(claim.Id);
        foreach(var op in new[]{"renew","complete","fail","reject"}) Assert.False(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,f.Token(claim),op));
        var next=(await f.Service.ClaimStockCheckerAsync("new-owner",900,3,f.JobId))!;
        Assert.Equal(claim.Id,next.Id);Assert.Equal(2,next.AttemptCount);
        Assert.False(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,f.Token(claim),"complete"));
    }

    [StockSqlFact]
    public async Task FailureAndCrashRetriesAreBounded_AndRetryDoesNotRunImmediately()
    {
        await using var f=new Fixture();await f.Start();await f.Row();
        for(var attempt=1;attempt<=3;attempt++) {
            var claim=(await f.Service.ClaimStockCheckerAsync("owner-"+attempt,900,3,f.JobId))!;
            Assert.Equal(attempt,claim.AttemptCount);
            Assert.True(await f.Service.UpdateStockCheckerLeaseAsync(claim.Id,f.Token(claim),"fail"));
            Assert.Null(await f.Service.ClaimStockCheckerAsync("early",900,3,f.JobId));
            Assert.Equal(attempt==3?"3":"-1",(await f.Read(claim.Id)).Confirmed);
            await f.Expire(claim.Id);
        }
        var crashed=await f.Row("-1",3);await f.Expire(crashed.Id);
        Assert.Null(await f.Service.ClaimStockCheckerAsync("after-crash",900,3,f.JobId));
        Assert.Equal("3",(await f.Read(crashed.Id)).Confirmed);
    }

    [StockSqlFact]
    public async Task ResultGuardRejectsStaleAndCrossJobWrites_AndRollsBackFailedAction()
    {
        await using var f=new Fixture();await f.Start(false);var row=await f.Row("-1",1);
        ActionExecutingContext Action(string owner="owner",int? job=null) {
            var http=new DefaultHttpContext();http.Request.Method="POST";
            http.Request.Headers["X-StockChecker-Entity"]=row.Id.ToString();http.Request.Headers["X-StockChecker-Attempt"]="1";
            http.Request.Headers["X-StockChecker-Worker"]=owner;
            return new(new ActionContext(http,new RouteData(),new ActionDescriptor()),new List<IFilterMetadata>(),
                new Dictionary<string,object?>{{"jobId",job??f.JobId}},new object());
        }
        foreach(var action in new[]{Action("wrong"),Action(job:f.JobId+1)}) {
            var invoked=false;
            await new StockCheckerLeaseFilter(f.Db).OnActionExecutionAsync(action,()=>{invoked=true;throw new Exception("Should be fenced");});
            Assert.False(invoked);Assert.NotNull(action.Result);
        }
        var valid=Action();
        await new StockCheckerLeaseFilter(f.Db).OnActionExecutionAsync(valid,async()=>{
            await f.Db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET last_error='ROLLBACK' WHERE id={row.Id}");
            return new ActionExecutedContext(valid,new List<IFilterMetadata>(),new object()){Result=new BadRequestResult()};
        });
        Assert.Null((await f.Read(row.Id)).LastError);
        valid=Action();
        await new StockCheckerLeaseFilter(f.Db).OnActionExecutionAsync(valid,async()=>{
            await f.Db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET last_error='COMMIT' WHERE id={row.Id}");
            return new ActionExecutedContext(valid,new List<IFilterMetadata>(),new object()){Result=new NoContentResult()};
        });
        Assert.Equal("COMMIT",(await f.Read(row.Id)).LastError);
    }

    [StockSqlFact]
    public async Task OwnerCannotChangeWhileResultWriteIsInProgress()
    {
        await using var f=new Fixture();await f.Start(false);var row=await f.Row("-1",1);
        var http=new DefaultHttpContext();http.Request.Method="POST";
        http.Request.Headers["X-StockChecker-Entity"]=row.Id.ToString();http.Request.Headers["X-StockChecker-Attempt"]="1";
        http.Request.Headers["X-StockChecker-Worker"]="owner";
        var action=new ActionExecutingContext(new ActionContext(http,new RouteData(),new ActionDescriptor()),new List<IFilterMetadata>(),
            new Dictionary<string,object?>{{"jobId",f.JobId}},new object());
        Task? takeover=null;
        await new StockCheckerLeaseFilter(f.Db).OnActionExecutionAsync(action,async()=>{
            takeover=Task.Run(async()=>{
                await using var other=new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>()
                    .UseSqlServer(Environment.GetEnvironmentVariable("STOCK_CHECKER_TEST_CONNECTION")).Options);
                await other.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET worker_id='next-owner',attempt_count=2 WHERE id={row.Id}");
            });
            await Task.Delay(200);Assert.False(takeover.IsCompleted);
            await f.Db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.model_numbers SET last_error='SAFE-WRITE' WHERE id={row.Id}");
            return new ActionExecutedContext(action,new List<IFilterMetadata>(),new object()){Result=new NoContentResult()};
        });
        await takeover!;
        Assert.Equal("next-owner",(await f.Read(row.Id)).WorkerId);
        Assert.Equal("SAFE-WRITE",(await f.Read(row.Id)).LastError);
    }
}
