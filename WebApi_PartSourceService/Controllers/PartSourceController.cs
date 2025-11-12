using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_PartSourceService.Services;

namespace WebApi_PartSourceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartSourceController : BaseControllerClass
    {

        public PartSourceController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get/{Id}")]
        public async Task<ActionResult<PartSource>> GetPartSourceById(int Id)
        {
            var job = await _databaseService.GetPartSourceById(Id);
            return Ok(job);
        }

        [HttpGet("getByUrl/{url}")]
        public async Task<ActionResult<PartSource>> GetPartSourceByUrl(string url)
        {
            var job = await _databaseService.GetPartSourceByUrl(url);
            return Ok(job);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<PartSource>>> GetPartSource()
        {
            var job = await _databaseService.GetPartSource();
            return Ok(job);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePartSource(int id, [FromBody] PartSource app)
        {
            if (app == null)
            {
                return BadRequest("Invalid PartSource.");
            }

            await _databaseService.UpdatePartSource(app);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPost("insert")]
        public async Task<IActionResult> AddPartSource([FromBody] PartSource request)
        {
            if (request == null )
            {
                return BadRequest("Некорректные данные."); // Возвращаем 400 Bad Request, если данные некорректны
            }

            try
            {
                // Вызов метода для вставки документа задания
                await _databaseService.AddPartSource(request);
                return Ok("Успешно вставлено."); // Возвращаем 200 OK с сообщением
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при вставке ."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePartSource([FromQuery] int Id)
        {
            if (Id <= 0 )
            {
                return BadRequest("Некорректные данные."); // Возвращаем 400 Bad Request, если данные некорректны
            }

            try
            {
                await _databaseService.DeletePartSource(Id);
                return Ok("Успешно удалено."); // Возвращаем 200 OK с сообщением
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при удалении ."); // Возвращаем 500 Internal Server Error
            }
        }

    }
}
