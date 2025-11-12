using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_JobStockService.Services;

namespace WebApi_JobStockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobStocksController : BaseControllerClass
    {

        public JobStocksController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        // Метод для удаления запасов по jobId
        [HttpDelete("delete/{jobId}")]
        public async Task<IActionResult> DeleteJobStocks(int jobId)
        {
            await _databaseService.DeleteJobStocksAsync(jobId);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpDelete("delete/{jobId}/{userStockId}")]
        public async Task<IActionResult> DeleteJobStocks(int jobId, int userStockId)
        {
            await _databaseService.DeleteJobStocksByUserIdAsync(jobId, userStockId);
            return NoContent(); // Возвращает 204 No Content
        }

        //!!!!!!!!вроде не используется
        //[HttpPut("update-status/{taskId}")]
        //public async Task<IActionResult> UpdateStockSearchStatus(int taskId, [FromBody] int status)
        //{
        //    // Вызов метода обновления статуса
        //    await _databaseService.UpdateStockSearchAsync(taskId, status);

        //    // Возвращаем 204 No Content, если обновление прошло успешно
        //    return NoContent();
        //}

        [HttpPost("insertJobStock")]
        public async Task<IActionResult> InsertJobStock([FromBody]  JobStock jobStockDto)
        {
            if (jobStockDto == null)
            {
                return BadRequest("Данные не могут быть пустыми."); // Возвращаем 400 Bad Request
            }

            try
            {
                await _databaseService.InsertJobStocksAsync(jobStockDto.JobId, jobStockDto.UserStockId, jobStockDto.MatchPartNumber);
                return CreatedAtAction(nameof(InsertJobStock), new { jobId = jobStockDto.JobId }, jobStockDto); // Возвращаем 201 Created
            }
            catch (DbUpdateException ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при вставке запасов работы."); // Возвращаем 500 Internal Server Error
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при вставке запасов работы."); // Возвращаем 500 Internal Server Error
            }
        }
    }
}
