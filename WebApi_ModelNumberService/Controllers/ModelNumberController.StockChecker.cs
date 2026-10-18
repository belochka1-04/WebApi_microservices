using Microsoft.AspNetCore.Mvc;
using WebApi_ModelNumberService.Services;

namespace WebApi_ModelNumberService.Controllers;

public partial class ModelNumberController
{
    [HttpGet("stock-checker/claim")]
    public async Task<IActionResult> ClaimStockChecker([FromQuery] string workerId,
        [FromQuery] int leaseSeconds=900,[FromQuery] int maxAttempts=3)
    {
        if(string.IsNullOrWhiteSpace(workerId)||workerId.Length>128) return BadRequest("Invalid owner token.");
        return Ok(await _databaseService.ClaimStockCheckerAsync(workerId,leaseSeconds,maxAttempts));
    }

    [HttpPost("stock-checker/{id:int}/{operation}")]
    public async Task<IActionResult> UpdateStockCheckerLease(int id,string operation,[FromBody] StockCheckerLeaseRequest request)
    {
        if(id<=0||request.AttemptCount<=0||string.IsNullOrWhiteSpace(request.WorkerId)||request.WorkerId.Length>128||
            operation is not ("renew" or "complete" or "reject" or "fail")) return BadRequest("Invalid lease request.");
        return await _databaseService.UpdateStockCheckerLeaseAsync(id,request,operation)
            ? NoContent():Conflict("Stock checker lease expired or owner changed.");
    }
}
