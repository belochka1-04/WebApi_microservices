using System.Data;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KameraData.Data;

// Hold the owner row lock until the result mutation commits in the same DbContext.
public sealed class StockCheckerLeaseFilter(KameraDbContext db) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request=context.HttpContext.Request;
        if(!request.Headers.ContainsKey("X-StockChecker-Entity")||HttpMethods.IsGet(request.Method)) { await next(); return; }
        if(!int.TryParse(request.Headers["X-StockChecker-Entity"],out var id)||id<=0||
           !int.TryParse(request.Headers["X-StockChecker-Attempt"],out var attempt)||attempt<=0||
           string.IsNullOrWhiteSpace(request.Headers["X-StockChecker-Worker"])) {
            context.Result=new BadRequestObjectResult("Invalid stock checker lease token."); return;
        }
        var ct=context.HttpContext.RequestAborted;
        await using var transaction=await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted,ct);
        await using var command=db.Database.GetDbConnection().CreateCommand();
        command.Transaction=transaction.GetDbTransaction();
        command.CommandText="""
SELECT job_id FROM dbo.model_numbers WITH (UPDLOCK,ROWLOCK)
WHERE id=@id AND confirmed='-1' AND worker_id=@owner AND attempt_count=@attempt
AND lease_until>SYSUTCDATETIME();
""";
        foreach(var pair in new (string Name,object Value)[]{("@id",id),("@attempt",attempt),("@owner",request.Headers["X-StockChecker-Worker"].ToString())}) {
            var p=command.CreateParameter(); p.ParameterName=pair.Name;p.Value=pair.Value;command.Parameters.Add(p);
        }
        var job=await command.ExecuteScalarAsync(ct);
        if(job==null) {context.Result=new ConflictObjectResult("Stock checker lease expired or owner changed.");return;}
        var jobId=Convert.ToInt32(job);
        // Reject a valid token used to mutate another job or another task's document metadata.
        foreach(var arg in context.ActionArguments) {
            if((arg.Key.Equals("jobId",StringComparison.OrdinalIgnoreCase)&&arg.Value is int routeJob&&routeJob!=jobId)||
               (arg.Value is JobStock stock&&stock.JobId!=jobId)||
               (arg.Value is JobDocInfo info&&(info.JobId!=jobId||info.ModelsNumberId!=id))) {
                context.Result=new BadRequestObjectResult("Lease does not belong to the target job/task."); return;
            }
        }
        var executed=await next();
        var code=(executed.Result as ObjectResult)?.StatusCode??(executed.Result as StatusCodeResult)?.StatusCode??context.HttpContext.Response.StatusCode;
        if(executed.Exception==null&&code<400) await transaction.CommitAsync(ct);
    }
}
