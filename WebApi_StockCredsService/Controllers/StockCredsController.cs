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
