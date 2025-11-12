using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsController : BaseControllerClass
    {

        public PartsController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("getPartsByModelId/{modelId}")]
        public async Task<IActionResult> GetPartsByModelId(int modelId)
        {
            if (modelId <= 0)
            {
                return BadRequest("Неверный идентификатор модели."); // Возвращаем 400 Bad Request
            }

            try
            {
                var parts = await _databaseService.GetPartsByModelIdAsync(modelId);

                if (parts == null || parts.Count == 0)
                {
                    return NotFound("Части не найдены для указанного идентификатора модели."); // Возвращаем 404 Not Found
                }

                return Ok(parts); // Возвращаем 200 OK с найденными частями
            }
            catch (DbUpdateException ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении частей."); // Возвращаем 500 Internal Server Error
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении частей."); // Возвращаем 500 Internal Server Error
            }
        }


    }
}
