using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_UserService.Services;

namespace WebApi_UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStocksController : BaseControllerClass
    {

        public UserStocksController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        [HttpGet("getUserStock/{userStockId}")]
        public async Task<ActionResult<UserStock>> GetUserStockById(int userStockId)
        {
            var job = await _databaseService.GetStockById(userStockId);
            return Ok(job);
        }

        [HttpGet("getStockById/{stockId}")]
        public async Task<ActionResult<StockCred>> GetStockById(int stockId)
        {
            var job = await _databaseService.GetStockCredById(stockId);
            return Ok(job);
        }

        [HttpGet("getUserStocksByStock/{stockId}/{userID}")]
        public async Task<ActionResult<StockCred>> GetUserStocksByStockCredAsync(int stockId, int userID)
        {
            var job = await _databaseService.GetUserStocksByStockCredAsync(stockId, userID);
            return Ok(job);
        }

        // Метод для вставки ответа
        [HttpPost("insert")]
        public async Task<IActionResult> InsertUserStockAsync([FromBody] UserStock request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            await _databaseService.InsertUserStockAsync(request);
            return NoContent(); // Возвращает 204 No Content
        }

        // Метод для удаления запасов по jobId
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteUserStocks(int Id)
        {
            await _databaseService.DeleteUserStocksAsync(Id);
            return NoContent(); // Возвращает 204 No Content
        }
    }
}
