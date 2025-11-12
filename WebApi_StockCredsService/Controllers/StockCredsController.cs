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


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStockCred(int id, [FromBody] StockCred stockCred)
        {
            if (stockCred == null)
            {
                return BadRequest("Invalid stockCred.");
            }

            await _databaseService.UpdateStockCreds(stockCred);
            return NoContent(); // Возвращает 204 No Content
        }
    }
}
