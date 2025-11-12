using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_SourceService.Services;

namespace WebApi_SourceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourceController : BaseControllerClass
    {

        public SourceController(IDatabaseService databaseService) : base(databaseService)
        {
        }



        [HttpGet("all")]
        public async Task<IActionResult> GetAllSources()
        {
            try
            {
                // Вызов метода для получения всех источников
                var sources = await _databaseService.GetAllSourcesAsync();

                if (sources == null || sources.Count == 0)
                {
                    return NotFound("Источники не найдены."); // Возвращаем 404, если источники не найдены
                }

                return Ok(sources); // Возвращаем 200 OK с данными
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении источников."); // Возвращаем 500 Internal Server Error
            }
        }

    } 
}
