using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_applications.Services;


namespace WebApi_applications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationLogController : BaseControllerClass
    {
        public ApplicationLogController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<ApplicationLog>>> GetApplicationLogList()
        {
            var result = await _databaseService.GetApplicationLogList();
            if (result == null || result.Count() == 0)
            {
                return NoContent(); // Возвращаем 204 No Content
            }
            return Ok(result);
        }

        [HttpPut("insert/{applicationName}")]
        public async Task<IActionResult> InsertApplicationLog( string applicationName, [FromBody] ApplicationLog app)
        {
            if (string.IsNullOrEmpty(app.RecType) || string.IsNullOrEmpty(applicationName) || string.IsNullOrEmpty(app.Text))
            {
                return BadRequest("ApplicationLog item cannot be null."); // 400 Bad Request
            }

            try
            {
                await _databaseService.InsertApplicationLogAsync(applicationName, app.RecType, app.Text, app.Description);
                return NoContent(); // 204 No Content
            }
            catch (ArgumentException ex) // Обработка ошибок валидации (например, некорректные данные в item)
            {
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (DatabaseException ex) // Обработка ошибок, связанных с базой данных
            {
                return StatusCode(500, "An error occurred while inserting the application log."); // 500 Internal Server Error
            }
            catch (Exception ex) // Обработка любых других непредвиденных исключений
            {
                return StatusCode(500, "An unexpected error occurred."); // 500 Internal Server Error
            }
        }

        // Определение исключений для более конкретной обработки ошибок
        public class DatabaseException : Exception
        {
            public DatabaseException(string message, Exception innerException = null) : base(message, innerException) { }
        }

    } 
}
