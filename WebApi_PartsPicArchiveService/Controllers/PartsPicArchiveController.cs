using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_PartsPicArchiveService.Services;

namespace WebApi_PartsPicArchiveService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsPicArchiveController : BaseControllerClass
    {

        public PartsPicArchiveController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get/{jobId}")]
        public async Task<ActionResult<PartsPicArchive>> GetPartsPicArchiveById(int jobId)
        {
            var job = await _databaseService.GetPartsPicArchiveById(jobId);
            return Ok(job);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePartsPicArchive(int id, [FromBody] PartsPicArchive app)
        {
            if (app == null)
            {
                return BadRequest("Invalid PartsPicArchive.");
            }

            await _databaseService.UpdatePartsPicArchive(app);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPost("insert")]
        public async Task<IActionResult> AddPartsPicArchive([FromBody] PartsPicArchive request)
        {
            if (request == null )
            {
                return BadRequest("Некорректные данные."); // Возвращаем 400 Bad Request, если данные некорректны
            }

            try
            {
                // Вызов метода для вставки документа задания
                await _databaseService.AddPartsPicArchive(request);
                return Ok("Запись  успешно вставленв."); // Возвращаем 200 OK с сообщением
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при вставке ."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePartsPicArchive([FromQuery] int Id)
        {
            if (Id <= 0 )
            {
                return BadRequest("Некорректные данные."); // Возвращаем 400 Bad Request, если данные некорректны
            }

            try
            {
                await _databaseService.DeletePartsPicArchive(Id);
                return Ok("Успешно удалено."); // Возвращаем 200 OK с сообщением
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при удалении "); // Возвращаем 500 Internal Server Error
            }
        }

    }
}
