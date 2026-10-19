using Microsoft.EntityFrameworkCore;

namespace WebApi_GoogleSearchTemplatesService.Services;

public sealed class GoogleRequestLeaseRequest
{
    public string WorkerId { get; set; } = "";
    public int AttemptCount { get; set; }
    public int LeaseSeconds { get; set; } = 900;
    public int RetryDelaySeconds { get; set; } = 2;
    public int MaxAttempts { get; set; } = 5;
    public string? Error { get; set; }
}

public partial class DatabaseService
{
    public async Task<bool> UpdateGoogleRequestLeaseAsync(int id, GoogleRequestLeaseRequest request, string operation)
    {
        var leaseSeconds = Math.Clamp(request.LeaseSeconds, 60, 3600);
        var retry = Math.Clamp(request.RetryDelaySeconds, 1, 86400);
        var max = Math.Clamp(request.MaxAttempts, 1, 100);
        var error = request.Error is { Length: > 1000 } ? request.Error[..1000] : request.Error;
        if (operation == "renew")
            return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.GoogleModelRequests SET LeaseUntil=DATEADD(second,{leaseSeconds},SYSUTCDATETIME())
WHERE Id={id} AND Status=2 AND WorkerId={request.WorkerId} AND AttemptCount={request.AttemptCount}
AND LeaseUntil > SYSUTCDATETIME();
""") == 1;
        if (operation == "complete")
            // Retain owner/attempt as a completion receipt, so a lost HTTP response can be retried.
            return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.GoogleModelRequests SET Status=1,LeaseUntil=NULL,LastError=NULL,LastCheckedAt=SYSUTCDATETIME()
WHERE Id={id} AND WorkerId={request.WorkerId} AND AttemptCount={request.AttemptCount}
AND ((Status=2 AND LeaseUntil > SYSUTCDATETIME()) OR Status=1);
""") == 1;
        if (operation != "fail") throw new ArgumentException("Unknown lease operation.", nameof(operation));
        return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.GoogleModelRequests SET
Status=CASE WHEN AttemptCount >= {max} THEN 3 ELSE 2 END,
LeaseUntil=CASE WHEN AttemptCount >= {max} THEN NULL ELSE DATEADD(second,{retry},SYSUTCDATETIME()) END,
WorkerId=NULL,LastError={error},LastCheckedAt=SYSUTCDATETIME()
WHERE Id={id} AND Status=2 AND WorkerId={request.WorkerId} AND AttemptCount={request.AttemptCount}
AND LeaseUntil > SYSUTCDATETIME();
""") == 1;
    }
}
