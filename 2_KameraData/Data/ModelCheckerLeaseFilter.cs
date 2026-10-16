using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KameraData.Data;

// The row lock fences each result mutation against concurrent claim/retry.
// The action and its writes use this same request-scoped DbContext/transaction.
public sealed class ModelCheckerLeaseFilter(KameraDbContext db) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        if (!request.Headers.ContainsKey("X-ModelChecker-Entity") || HttpMethods.IsGet(request.Method))
        {
            await next();
            return;
        }
        if (!int.TryParse(request.Headers["X-ModelChecker-Entity"], out var id) || id <= 0 ||
            !int.TryParse(request.Headers["X-ModelChecker-Attempt"], out var attempt) || attempt <= 0 ||
            string.IsNullOrWhiteSpace(request.Headers["X-ModelChecker-Worker"]))
        {
            context.Result = new BadRequestObjectResult("Invalid model checker lease token.");
            return;
        }
        var ct = context.HttpContext.RequestAborted;
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandTimeout = 90;
        command.CommandText = @"SELECT 1 FROM dbo.job_description_and_notes WITH (UPDLOCK, ROWLOCK)
WHERE id = @Id AND status = '2' AND worker_id = @WorkerId AND attempt_count = @Attempt
  AND lease_until_utc > SYSUTCDATETIME();";
        foreach (var pair in new (string Name, object Value)[] {
            ("@Id", id), ("@Attempt", attempt), ("@WorkerId", request.Headers["X-ModelChecker-Worker"].ToString()) })
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = pair.Name; parameter.Value = pair.Value;
            command.Parameters.Add(parameter);
        }
        if (await command.ExecuteScalarAsync(ct) == null)
        {
            context.Result = new ConflictObjectResult("Model checker lease expired or ownership changed.");
            return;
        }
        var executed = await next();
        var code = (executed.Result as ObjectResult)?.StatusCode
            ?? (executed.Result as StatusCodeResult)?.StatusCode ?? context.HttpContext.Response.StatusCode;
        if (executed.Exception == null && code < 400) await transaction.CommitAsync(ct);
    }
}
