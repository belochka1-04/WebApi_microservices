using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_StockCredsService.Services;

namespace WebApi_StockCredsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockCredsController : BaseControllerClass
    {
        public StockCredsController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<StockCred>>> GetStockCreds()
        {
            var job = await _databaseService.GetStockCreds();
            return Ok(job);
        }

        [HttpGet("claim-due")]
        public async Task<ActionResult<List<StockCred>>> ClaimDueStockCreds(
            [FromQuery] string workerId,
            [FromQuery] int batchSize = 10,
            [FromQuery] int leaseSeconds = 900)
        {
            if (string.IsNullOrWhiteSpace(workerId))
            {
                return BadRequest("workerId is required.");
            }

            var stockCreds = await _databaseService.ClaimDueStockCreds(workerId, batchSize, leaseSeconds);
            return Ok(stockCreds);
        }

        [HttpPost("release-lease/{id}")]
        public async Task<IActionResult> ReleaseStockCredLease(int id, [FromQuery] string workerId)
        {
            if (string.IsNullOrWhiteSpace(workerId))
            {
                return BadRequest("workerId is required.");
            }

            await _databaseService.ReleaseStockCredLease(id, workerId);
            return NoContent();
        }

        [HttpPost("renew-sync/{id}")]
        public async Task<IActionResult> RenewSync(int id, [FromBody] StockSyncLeaseRequest request)
        {
            if (id <= 0 || request.LockId == Guid.Empty || string.IsNullOrWhiteSpace(request.WorkerId))
                return BadRequest("Invalid stock sync lease.");
            return await _databaseService.UpdateSyncLease(id, request, false)
                ? NoContent() : Conflict("Stock sync lease expired or owner changed.");
        }

        [HttpPost("finish-sync/{id}")]
        public async Task<IActionResult> FinishSync(int id, [FromBody] StockSyncLeaseRequest request)
        {
            if (id <= 0 || request.LockId == Guid.Empty || string.IsNullOrWhiteSpace(request.WorkerId))
                return BadRequest("Invalid stock sync lease.");
            return await _databaseService.UpdateSyncLease(id, request, true)
                ? NoContent() : Conflict("Stock sync lease expired or owner changed.");
        }

        [HttpPut("sync-state/{id}")]
        public async Task<IActionResult> UpdateSyncState(int id, [FromBody] StockSyncStateRequest request)
        {
            if (!Request.Headers.ContainsKey("X-StockSync-Stock")) return BadRequest("Stock sync lease required.");
            await _databaseService.UpdateSyncState(id, request);
            return NoContent();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStockCred(int id, [FromBody] StockCred stockCred)
        {
            if (stockCred == null)
            {
                return BadRequest("Invalid stockCred.");
            }

            await _databaseService.UpdateStockCreds(stockCred);
            return NoContent();
        }
    }
}
