using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_UniqModelService.Services;

namespace WebApi_UniqModelService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniqModelController :  BaseControllerClass
    {
        
        public UniqModelController(IDatabaseService databaseService) : base(databaseService)
    {
    }

    [HttpPost("getByModels")]
        public async Task<IActionResult> GetByModels([FromBody]  List<string> models)
        {
            if (models is null)
            {
                return BadRequest("Неверный идентификатор модели."); // Возвращаем 400 Bad Request
            }

            try
            {
                var result = await _databaseService.GetUniqModelByModels(models);

                if (result == null || result.Count == 0)
                {
                    return NotFound("Не найдены модели."); // Возвращаем 404 Not Found
                }

                return Ok(result); // Возвращаем 200 OK с найденными частями
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
