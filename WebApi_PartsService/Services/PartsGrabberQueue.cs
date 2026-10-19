using Dapper;
using KameraData.Data.Models;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace WebApi_PartsService.Services;

public sealed class PartsGrabberRequest
{
    public string WorkerId {get;set;}="";
    public int AttemptCount {get;set;}
    public int BatchSize {get;set;}=1;
    public int LeaseSeconds {get;set;}=3600;
    public int MaxAttempts {get;set;}=5;
    public int RetryDelaySeconds {get;set;}=300;
    public string? Error {get;set;}
    public JsonElement Data {get;set;}
}

public sealed class PartsGrabberQueue(string connection,SqlConnection? session=null,SqlTransaction? sessionTransaction=null)
{
    public async Task<IEnumerable<PartsAndReplace>> ClaimAsync(PartsGrabberRequest request,int? onlyId=null)
    {
        await using var owned=session==null?new SqlConnection(connection):null;
        var db=owned??session!;if(db.State!=System.Data.ConnectionState.Open)await db.OpenAsync();
        await db.ExecuteAsync("""
UPDATE dbo.parts_and_replaces SET status='3',parts_worker_id=NULL,parts_lease_until_utc=NULL,
parts_processing_completed_at_utc=SYSUTCDATETIME(),parts_processing_last_error=COALESCE(parts_processing_last_error,'PartsGrabber attempts exhausted after lease expiry.')
WHERE (@OnlyId IS NULL OR id=@OnlyId) AND ISNULL(parts_attempt_count,0)>=@MaxAttempts
AND (status='0' OR(status='1' AND parts_lease_until_utc<SYSUTCDATETIME()));
""",new{OnlyId=onlyId,MaxAttempts=Math.Clamp(request.MaxAttempts,1,100)},sessionTransaction);
        return await db.QueryAsync<PartsAndReplace>(PartsGrabberSql.Claim,new{request.WorkerId,
            BatchSize=Math.Clamp(request.BatchSize,1,100),LeaseSeconds=Math.Clamp(request.LeaseSeconds,60,21600),
            MaxAttempts=Math.Clamp(request.MaxAttempts,1,100),OnlyId=onlyId},sessionTransaction);
    }

    public async Task<bool> ExecuteAsync(int id,string operation,PartsGrabberRequest request)
    {
        await using var owned=session==null?new SqlConnection(connection):null;
        var db=owned??session!;if(db.State!=System.Data.ConnectionState.Open)await db.OpenAsync();
        await using var ownedTransaction=sessionTransaction==null?await db.BeginTransactionAsync():null;
        var tx=sessionTransaction??ownedTransaction!;
        var ownedId=await db.ExecuteScalarAsync<int?>("""
SELECT id FROM dbo.parts_and_replaces WITH(UPDLOCK,ROWLOCK)
WHERE id=@id AND parts_worker_id=@WorkerId AND parts_attempt_count=@AttemptCount
AND ((status='1' AND parts_lease_until_utc>SYSUTCDATETIME())
 OR(@operation='complete' AND status IN('2','3') AND parts_processing_completed_at_utc IS NOT NULL));
""",new{id,request.WorkerId,request.AttemptCount,operation},tx);
        if(ownedId==null)return false;
        if(operation=="renew") await db.ExecuteAsync("UPDATE dbo.parts_and_replaces SET parts_lease_until_utc=DATEADD(second,@seconds,SYSUTCDATETIME()) WHERE id=@id",
            new{id,seconds=Math.Clamp(request.LeaseSeconds,60,21600)},tx);
        else if(operation=="fail") await db.ExecuteAsync("""
UPDATE dbo.parts_and_replaces SET status=CASE WHEN parts_attempt_count>=@max THEN '3' ELSE '1' END,
parts_worker_id=NULL,parts_lease_until_utc=CASE WHEN parts_attempt_count>=@max THEN NULL ELSE DATEADD(second,@delay,SYSUTCDATETIME()) END,
parts_processing_last_error=@error,parts_processing_completed_at_utc=CASE WHEN parts_attempt_count>=@max THEN SYSUTCDATETIME() ELSE NULL END
WHERE id=@id;
""",new{id,max=Math.Clamp(request.MaxAttempts,1,100),delay=Math.Clamp(request.RetryDelaySeconds,1,86400),
            error=request.Error is {Length:>2000}?request.Error[..2000]:request.Error},tx);
        else if(operation=="complete") {
            var result=Read<PartsAndReplace>(request);
            if(result.Id!=id||result.Status is not("2" or "3"))throw new ArgumentException("Invalid final part result.");
            await db.ExecuteAsync("""
UPDATE dbo.parts_and_replaces SET part_name=@PartName,replaces=@Replaces,pic=@Pic,Photo_status=@PhotoStatus,
status=@Status,date_update=SYSUTCDATETIME(),parts_lease_until_utc=NULL,
parts_processing_completed_at_utc=SYSUTCDATETIME(),parts_processing_last_error=NULL WHERE id=@Id;
""",result,tx);
        }
        else if(operation=="name") {
            var row=Read<PartsNamesArchive>(request);if(row.PartsAndReplacesId!=id)throw new ArgumentException("Cross-part write.");
            await db.ExecuteAsync(PartsGrabberSql.Name,row,tx);
        }
        else if(operation=="replace") {
            var row=Read<ReplacesArchive>(request);if(row.PartsAndReplacesId!=id)throw new ArgumentException("Cross-part write.");
            await db.ExecuteAsync(PartsGrabberSql.Replace,row,tx);
        }
        else if(operation=="picture") {
            var row=Read<PartsPicArchive>(request);if(row.PartsAndReplacesId!=id)throw new ArgumentException("Cross-part write.");
            await db.ExecuteAsync(PartsGrabberSql.Picture,row,tx);
        }
        else throw new ArgumentException("Unknown PartsGrabber operation.");
        if(ownedTransaction!=null)await ownedTransaction.CommitAsync();return true;
    }
    private static T Read<T>(PartsGrabberRequest request)=>request.Data.Deserialize<T>(new JsonSerializerOptions{PropertyNameCaseInsensitive=true})
        ??throw new ArgumentException("Missing write data.");
}
