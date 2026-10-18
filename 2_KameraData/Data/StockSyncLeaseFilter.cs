using System.Data;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KameraData.Data;

// Optional fencing for SampleStocksCore writes; ordinary bot operations keep their existing path.
public sealed class StockSyncLeaseFilter(KameraDbContext db) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        if (HttpMethods.IsGet(request.Method) || !request.Headers.ContainsKey("X-StockSync-Stock"))
        { await next(); return; }
        if (!int.TryParse(request.Headers["X-StockSync-Stock"], out var stockId) || stockId <= 0 ||
            !Guid.TryParse(request.Headers["X-StockSync-Lock"], out var lockId) || lockId == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.Headers["X-StockSync-Worker"]))
        { context.Result = new BadRequestObjectResult("Invalid stock sync lease."); return; }
        if (context.ActionArguments.Values.OfType<UserStock>().Any(row => row.StockId != stockId) ||
            context.ActionArguments.Values.OfType<StockCred>().Any(stock => stock.Id != stockId) ||
            (context.RouteData.Values["controller"]?.ToString() == "StockCreds" &&
             context.ActionArguments.TryGetValue("id", out var routeId) && !Equals(routeId, stockId)))
        { context.Result = new BadRequestObjectResult("Stock does not match sync lease."); return; }

        var ct = context.HttpContext.RequestAborted;
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandText = """
SELECT 1 FROM dbo.stock_creds WITH (UPDLOCK, ROWLOCK)
WHERE id=@Id AND sync_lock_id=@LockId AND sync_locked_by=@Worker
AND sync_lease_until > SYSUTCDATETIME();
""";
        foreach (var pair in new (string Name, object Value)[] {
            ("@Id", stockId), ("@LockId", lockId), ("@Worker", request.Headers["X-StockSync-Worker"].ToString()) })
        {
            var p = command.CreateParameter(); p.ParameterName = pair.Name; p.Value = pair.Value;
            command.Parameters.Add(p);
        }
        if (await command.ExecuteScalarAsync(ct) == null)
        { context.Result = new ConflictObjectResult("Stock sync lease expired or owner changed."); return; }
        if (HttpMethods.IsDelete(request.Method) && context.ActionArguments.TryGetValue("Id", out var value) && value is int rowId &&
            !await db.UserStocks.AnyAsync(row => row.Id == rowId && row.StockId == stockId, ct))
        { context.Result = new NotFoundObjectResult("Stock row does not belong to this sync lease."); return; }
        var executed = await next();
        var status = (executed.Result as ObjectResult)?.StatusCode ?? (executed.Result as StatusCodeResult)?.StatusCode
            ?? context.HttpContext.Response.StatusCode;
        if (executed.Exception == null && status < 400) await transaction.CommitAsync(ct);
    }
}
