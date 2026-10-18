using KameraData.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebApi_ModelNumberService.Services;

public sealed class StockCheckerLeaseRequest
{
    public string WorkerId { get; set; } = "";
    public int AttemptCount { get; set; }
    public int LeaseSeconds { get; set; } = 900;
    public int MaxAttempts { get; set; } = 3;
    public int RetryDelaySeconds { get; set; } = 2;
    public string? Error { get; set; }
}

public partial class DatabaseService
{
    // jobId is an optional service-level scope for isolated verification, not exposed by the polling endpoint.
    public async Task<ModelNumber?> ClaimStockCheckerAsync(string owner, int leaseSeconds, int maxAttempts, int? jobId = null)
    {
        var rows = await _dbContext.ModelNumbers.FromSqlRaw("""
SET XACT_ABORT ON;
DECLARE @claimed TABLE(id int PRIMARY KEY);
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @lockResult int;
 EXEC @lockResult = sys.sp_getapplock @Resource=N'StockCheckerDb:claim', @LockMode='Exclusive',
      @LockOwner='Transaction', @LockTimeout=5000;
 IF @lockResult < 0 THROW 51000, 'Cannot acquire stock checker admission lock.', 1;

 UPDATE dbo.model_numbers WITH (ROWLOCK)
 SET confirmed='3',worker_id=NULL,lease_until=NULL,
     last_error=COALESCE(last_error,'StockCheckerDb attempts exhausted after lease expiry.')
 WHERE (@jobId IS NULL OR job_id=@jobId) AND ISNULL(attempt_count,0)>=@maxAttempts
 AND (confirmed='1' OR (confirmed='-1' AND lease_until<SYSUTCDATETIME()));

 ;WITH next_row AS (
 SELECT TOP(1) mn.* FROM dbo.model_numbers mn WITH (UPDLOCK,READPAST,ROWLOCK)
 WHERE (@jobId IS NULL OR mn.job_id=@jobId) AND mn.jd_confirmed=1
 AND ISNULL(mn.attempt_count,0)<@maxAttempts
 AND (mn.confirmed='1' OR (mn.confirmed='-1' AND mn.lease_until<SYSUTCDATETIME()))
 AND NOT EXISTS (SELECT 1 FROM dbo.model_numbers busy
     WHERE busy.job_id=mn.job_id AND busy.id<>mn.id AND busy.confirmed='-1'
     AND busy.lease_until>SYSUTCDATETIME())
 ORDER BY mn.id)
 UPDATE next_row SET confirmed='-1',worker_id=@owner,got_for_work_at=SYSUTCDATETIME(),
 lease_until=DATEADD(second,@leaseSeconds,SYSUTCDATETIME()),
 attempt_count=ISNULL(attempt_count,0)+1,last_error=NULL
 OUTPUT inserted.id INTO @claimed;
 COMMIT;
 SELECT mn.* FROM dbo.model_numbers mn JOIN @claimed c ON c.id=mn.id;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK;
 THROW;
END CATCH
""", new SqlParameter("@owner", owner), new SqlParameter("@leaseSeconds", Math.Clamp(leaseSeconds,60,3600)),
            new SqlParameter("@maxAttempts",Math.Clamp(maxAttempts,1,100)),
            new SqlParameter("@jobId",(object?)jobId ?? DBNull.Value)).AsNoTracking().ToListAsync();
        return rows.SingleOrDefault();
    }

    public async Task<bool> UpdateStockCheckerLeaseAsync(int id, StockCheckerLeaseRequest request, string operation)
    {
        var lease = Math.Clamp(request.LeaseSeconds,60,3600);
        var retry = Math.Clamp(request.RetryDelaySeconds,1,3600);
        var max = Math.Clamp(request.MaxAttempts,1,100);
        var error = request.Error is {Length:>1000} ? request.Error[..1000] : request.Error;
        if(operation=="renew") return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.model_numbers SET lease_until=DATEADD(second,{lease},SYSUTCDATETIME())
WHERE id={id} AND confirmed='-1' AND worker_id={request.WorkerId} AND attempt_count={request.AttemptCount}
AND lease_until>SYSUTCDATETIME();
""")==1;
        if(operation is "complete" or "reject") {
            var status=operation=="complete" ? "2" : "3";
            return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.model_numbers SET confirmed={status},lease_until=NULL,last_error={error}
WHERE id={id} AND worker_id={request.WorkerId} AND attempt_count={request.AttemptCount}
AND ((confirmed='-1' AND lease_until>SYSUTCDATETIME()) OR confirmed={status});
""")==1;
        }
        if(operation!="fail") throw new ArgumentException("Unknown lease operation.");
        return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.model_numbers SET confirmed=CASE WHEN attempt_count>={max} THEN '3' ELSE '-1' END,
worker_id=NULL,lease_until=CASE WHEN attempt_count>={max} THEN NULL ELSE DATEADD(second,{retry},SYSUTCDATETIME()) END,
last_error={error}
WHERE id={id} AND confirmed='-1' AND worker_id={request.WorkerId} AND attempt_count={request.AttemptCount}
AND lease_until>SYSUTCDATETIME();
""")==1;
    }
}
