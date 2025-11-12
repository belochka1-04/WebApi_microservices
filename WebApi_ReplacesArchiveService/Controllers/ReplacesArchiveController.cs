using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ReplacesArchiveService.Services;

namespace WebApi_ReplacesArchiveService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReplacesArchiveController : BaseControllerClass
    {

        public ReplacesArchiveController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get/{Id}")]
        public async Task<ActionResult<ReplacesArchive>> GetReplacesArchiveById(int Id)
        {
            var job = await _databaseService.GetReplacesArchiveById(Id);
            return Ok(job);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateReplacesArchive(int id, [FromBody] ReplacesArchive app)
        {
            if (app == null)
            {
                return BadRequest("Invalid app.");
            }

            await _databaseService.UpdateReplacesArchive(app);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPost("insert")]
        public async Task<IActionResult> AddReplacesArchive([FromBody] ReplacesArchive request)
        {
            if (request == null )
            {
                return BadRequest("Некорректные данные."); // Возвращаем 400 Bad Request, если данные некорректны
            }

            try
            {
                // Вызов метода для вставки документа задания
                await _databaseService.AddReplacesArchive(request);
                return Ok("Успешно вставлено."); // Возвращаем 200 OK с сообщением
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при вставке ."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReplacesArchive([FromQuery] int Id)
        {
            if (Id <= 0 )
            {
                return BadRequest("Некорректные данные."); // Возвращаем 400 Bad Request, если данные некорректны
            }

            try
            {
                await _databaseService.DeleteReplacesArchive(Id);
                return Ok("Успешно удалено."); // Возвращаем 200 OK с сообщением
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при удалении."); // Возвращаем 500 Internal Server Error
            }
        }

    }
}
