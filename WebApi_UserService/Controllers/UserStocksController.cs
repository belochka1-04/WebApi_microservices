using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_UserService.Services;

namespace WebApi_UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        // GET /api/userstocks/by-part/{partId}?userId={userId}
        [HttpGet("by-part/{partId}")]
        public async Task<ActionResult<List<UserStock>>> GetByPart(int partId, [FromQuery] int userId)
        {
            if (userId == 0) return BadRequest("userId required");

            var stocks = await _databaseService.GetUserStocksByPartAsync(partId, userId);
            return Ok(stocks);
        }

        // GET /api/userstocks/by-part/{partId}/all - для внутреннего использования (без фильтра по userId)
        [HttpGet("by-part/{partId}/all")]
        public async Task<ActionResult<List<UserStock>>> GetAllByPart(int partId)
        {
            var stocks = await _databaseService.GetAllUserStocksByPartAsync(partId);
            return Ok(stocks);
        }

        // POST /api/userstocks/by-parts (для массовой загрузки)
        [HttpPost("by-parts")]
        public async Task<ActionResult<List<UserStock>>> GetByParts([FromBody] List<int> partIds, [FromQuery] int userId)
        {
            if (userId == 0) return BadRequest("userId required");

            var stocks = await _databaseService.GetUserStocksByPartsAsync(partIds, userId);
            return Ok(stocks);
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
