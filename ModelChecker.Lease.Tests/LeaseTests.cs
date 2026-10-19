using KameraData.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WebApi_JobService.Services;
using Xunit;
using Service = WebApi_JobService.Services.DatabaseService;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace ModelChecker.Lease.Tests;
public sealed class SqlFactAttribute : FactAttribute
{
    public SqlFactAttribute() { if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MNC_TEST_CONNECTION"))) Skip="Requires an explicitly configured test database."; }
}
public sealed class LeaseTests
{
    [SqlFact]
    public async Task DatabaseHealthCheckUsesAValidLightweightQuery()
    {
        await using var db=new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>()
            .UseSqlServer(Environment.GetEnvironmentVariable("MNC_TEST_CONNECTION")).Options);
        var service=new Service(db,null!,NullLogger<Service>.Instance);
        Assert.True(await service.IsDatabaseHealthyAsync());
    }
    private sealed class Fixture : IDisposable
    {
        public readonly string Connection=Environment.GetEnvironmentVariable("MNC_TEST_CONNECTION")!;
        public readonly int JobId;
        public readonly int Id;
        public readonly string Owner="qa-lease:"+Guid.NewGuid().ToString("N");
        public Fixture()
        {
            JobId=Convert.ToInt32(Sql("INSERT dbo.jobs(user_id,job_number,update_request_status) OUTPUT inserted.id VALUES(1036,'QA-LEASE-'+CONVERT(varchar(36),NEWID()),'9')"));
            Id=Convert.ToInt32(Sql("DECLARE @ids TABLE(id int); INSERT dbo.job_description_and_notes(job_id,job_description,job_notes,status,google_confirmed,worker_id,lease_until_utc,attempt_count) OUTPUT inserted.id INTO @ids VALUES(@job,'QA lease test','','2',0,@owner,DATEADD(MINUTE,10,SYSUTCDATETIME()),1); SELECT id FROM @ids",("@job",JobId),("@owner",Owner)));
            Sql("UPDATE dbo.job_description_and_notes SET google_confirmed=0 WHERE id=@id",("@id",Id));
        }
        public object? Sql(string sql,params (string Name,object Value)[] values)
        {
            using var c=new SqlConnection(Connection); c.Open(); using var command=new SqlCommand(sql,c) {CommandTimeout=10};
            foreach(var pair in values) command.Parameters.AddWithValue(pair.Name,pair.Value);
            return command.ExecuteScalar();
        }
        public KameraDbContext Context()=>new(new DbContextOptionsBuilder<KameraDbContext>().UseSqlServer(Connection).Options);
        public Service Service(KameraDbContext db)=>new(db,null!,NullLogger<Service>.Instance);
        public ModelCheckerLeaseRequest Request(int attempt=1,string? owner=null)=>new(){WorkerId=owner??Owner,AttemptCount=attempt,LeaseSeconds=120,Error="test failure"};
        public ActionExecutingContext Action(string? owner=null,int attempt=1)
        {
            var http=new DefaultHttpContext(); http.Request.Method="POST";
            http.Request.Headers["X-ModelChecker-Entity"]=Id.ToString();
            http.Request.Headers["X-ModelChecker-Attempt"]=attempt.ToString();
            http.Request.Headers["X-ModelChecker-Worker"]=owner??Owner;
            return new(new ActionContext(http,new RouteData(),new ActionDescriptor()),new List<IFilterMetadata>(),new Dictionary<string,object?>(),new object());
        }
        public static ActionExecutedContext Done(ActionExecutingContext action,IActionResult? result=null)=>new(action,new List<IFilterMetadata>(),new object()){Result=result??new NoContentResult()};
        public void Dispose()=>Sql("DELETE d FROM dbo.job_description_and_notes d JOIN dbo.jobs j ON j.id=d.job_id WHERE j.id=@id AND j.user_id=1036 AND j.job_number LIKE 'QA-LEASE-%'; DELETE dbo.jobs WHERE id=@id AND user_id=1036 AND job_number LIKE 'QA-LEASE-%'",("@id",JobId));
    }

    [SqlFact]
    public async Task OwnerAndAttemptAreRequired_ForRenewCompleteAndFailure()
    {
        using var f=new Fixture();await using var db=f.Context();var service=f.Service(db);
        foreach(var operation in new[]{"renew","complete","fail"}) {
            Assert.False(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(owner:"another-owner"),operation));
            Assert.False(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(attempt:2),operation));
        }
        Assert.True(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(),"renew"));
        Assert.True(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(),"complete"));
        Assert.Equal("1",f.Sql("SELECT status FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
        Assert.False(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(),"complete"));
    }

    [SqlFact]
    public async Task ThreeFailures_StopAutomaticRetries_AndManualRetryResetsCounterButKeepsCause()
    {
        using var f=new Fixture();await using var db=f.Context();var service=f.Service(db);
        for(var attempt=1;attempt<=3;attempt++) {
            f.Sql("UPDATE dbo.job_description_and_notes SET status='2',attempt_count=@attempt,worker_id=@owner,lease_until_utc=DATEADD(MINUTE,10,SYSUTCDATETIME()) WHERE id=@id",("@attempt",attempt),("@owner",f.Owner),("@id",f.Id));
            Assert.True(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(attempt),"fail"));
            Assert.Equal(attempt<3?"0":"3",f.Sql("SELECT status FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
            Assert.Equal("test failure",f.Sql("SELECT processing_last_error FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
            if(attempt<3) Assert.True(Convert.ToInt32(f.Sql("SELECT CASE WHEN lease_until_utc>SYSUTCDATETIME() THEN 1 ELSE 0 END FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)))==1);
        }
        Assert.True(await service.RetryModelCheckerJobAsync(f.Id,"Corrected source data"));
        Assert.Equal(0,f.Sql("SELECT attempt_count FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
        Assert.Equal("test failure",f.Sql("SELECT processing_last_error FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
        Assert.False(await service.RetryModelCheckerJobAsync(f.Id,"duplicate"));
        f.Sql("UPDATE dbo.job_description_and_notes SET status='2',attempt_count=1,worker_id='new-claim',lease_until_utc=DATEADD(MINUTE,10,SYSUTCDATETIME()) WHERE id=@id",("@id",f.Id));
        Assert.False(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(),"complete"));
    }

    [SqlFact]
    public async Task ExpiredLeaseCannotBeRenewedCompletedOrFailed()
    {
        using var f=new Fixture();await using var db=f.Context();var service=f.Service(db);
        f.Sql("UPDATE dbo.job_description_and_notes SET lease_until_utc=DATEADD(MINUTE,-1,SYSUTCDATETIME()) WHERE id=@id",("@id",f.Id));
        foreach(var operation in new[]{"renew","complete","fail"}) Assert.False(await service.UpdateModelCheckerLeaseAsync(f.Id,f.Request(),operation));
    }

    [SqlFact]
    public async Task ResultGuardRejectsStaleOwnerAttemptAndExpiredLease_BeforeActionRuns()
    {
        using var f=new Fixture();
        foreach(var action in new[]{f.Action("wrong"),f.Action(attempt:2)}) {
            await using var db=f.Context();var invoked=false;
            await new ModelCheckerLeaseFilter(db).OnActionExecutionAsync(action,()=>{invoked=true;return Task.FromResult(Fixture.Done(action));});
            Assert.False(invoked);Assert.IsType<ConflictObjectResult>(action.Result);
        }
        f.Sql("UPDATE dbo.job_description_and_notes SET lease_until_utc=DATEADD(MINUTE,-1,SYSUTCDATETIME()) WHERE id=@id",("@id",f.Id));
        await using(var db=f.Context()) {
            var action=f.Action();var invoked=false;
            await new ModelCheckerLeaseFilter(db).OnActionExecutionAsync(action,()=>{invoked=true;return Task.FromResult(Fixture.Done(action));});
            Assert.False(invoked);Assert.IsType<ConflictObjectResult>(action.Result);
        }
    }

    [SqlFact]
    public async Task OwnershipCannotChangeBetweenValidationAndResultCommit()
    {
        using var f=new Fixture();await using var db=f.Context();var action=f.Action();Task<object?>? takeover=null;
        await new ModelCheckerLeaseFilter(db).OnActionExecutionAsync(action,async()=>{
            takeover=Task.Run(()=>f.Sql("UPDATE dbo.job_description_and_notes SET worker_id='next-owner',attempt_count=2 WHERE id=@id; SELECT 1",("@id",f.Id)));
            await Task.Delay(300);Assert.False(takeover.IsCompleted);
            await db.Database.ExecuteSqlRawAsync("UPDATE dbo.job_description_and_notes SET model='FENCED' WHERE id={0}",f.Id);
            return Fixture.Done(action);
        });
        await takeover!;
        Assert.Equal("FENCED",f.Sql("SELECT model FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
        await using var staleDb=f.Context();var stale=f.Action();var invoked=false;
        await new ModelCheckerLeaseFilter(staleDb).OnActionExecutionAsync(stale,()=>{invoked=true;return Task.FromResult(Fixture.Done(stale));});
        Assert.False(invoked);Assert.IsType<ConflictObjectResult>(stale.Result);
    }

    [SqlFact]
    public async Task ActionFailureRollsBackResultMutation()
    {
        using var f=new Fixture();await using var db=f.Context();var action=f.Action();
        await new ModelCheckerLeaseFilter(db).OnActionExecutionAsync(action,async()=>{
            await db.Database.ExecuteSqlRawAsync("UPDATE dbo.job_description_and_notes SET model='SHOULD-ROLL-BACK' WHERE id={0}",f.Id);
            return Fixture.Done(action,new BadRequestResult());
        });
        Assert.Equal(DBNull.Value,f.Sql("SELECT model FROM dbo.job_description_and_notes WHERE id=@id",("@id",f.Id)));
    }
}
