using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsAndReplacesController : BaseControllerClass
    {

        public PartsAndReplacesController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartsAndReplace>> GetById(int id)
        {
            var part = await _databaseService.Get(id); 
            if (part == null) return NotFound();
            return Ok(part);
        }

        [HttpPost]
        public async Task<ActionResult<PartsAndReplace>> Create([FromBody] CreatePartDto dto)
        {
            // Создаем и сразу получаем объект с ID
            var created = await _databaseService.CreatePartAsync(dto.PartNumber);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPost("getPartsAndReplaces")]
        public async Task<ActionResult<List<PartsAndReplace>>> GetPartsAndReplaces([FromBody] List<int> replaceIds)
        {
            if (replaceIds == null || replaceIds.Count == 0)
            {
                return BadRequest("Список идентификаторов замен не может быть пустым."); // Возвращаем 400 Bad Request
            }

            try
            {
                var partsAndReplaces = await _databaseService.GetPartsAndReplacesAsync(replaceIds);

                if (partsAndReplaces == null || partsAndReplaces.Count == 0)
                {
                    return NotFound("Детали замен не найдены."); // Возвращаем 404 Not Found, если ничего не найдено
                }

                return Ok(partsAndReplaces); // Возвращаем 200 OK с найденными деталями
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении деталей замен."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("getPartsWithState/{state}")]
        public async Task<ActionResult<List<PartsAndReplace>>> GetPartsAndReplacesWithStateAsync(string state)
        {
            if (state == null )
            {
                return BadRequest("Статус не может быть пустым."); // Возвращаем 400 Bad Request
            }

            try
            {
                var partsAndReplaces = await _databaseService.GetPartsAndReplacesWithStateAsync(state);

                if (partsAndReplaces == null || partsAndReplaces.Count == 0)
                {
                    return NotFound("Детали замен не найдены."); // Возвращаем 404 Not Found, если ничего не найдено
                }

                return Ok(partsAndReplaces); // Возвращаем 200 OK с найденными деталями
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении деталей замен."); // Возвращаем 500 Internal Server Error
            }
        }

        [HttpGet("GetPartsAndReplacesByUserID/{userID}")]
        public async Task<ActionResult<List<PartsAndReplace>>> GetPartsAndReplacesByUserIDAsync(int userID)
        {
            if (userID == 0)
            {
                return BadRequest("userID не может быть пустым."); // Возвращаем 400 Bad Request
            }

            try
            {
                var partsAndReplaces = await _databaseService.GetPartsAndReplacesByUserIDAsync(userID);

                if (partsAndReplaces == null || partsAndReplaces.Count == 0)
                {
                    return NotFound("Детали замен не найдены."); // Возвращаем 404 Not Found, если ничего не найдено
                }

                return Ok(partsAndReplaces); // Возвращаем 200 OK с найденными деталями
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                return StatusCode(500, "Ошибка сервера при получении деталей замен."); // Возвращаем 500 Internal Server Error
            }
        }

        //[HttpPost("insertPartsAndReplaces")]
        //public async Task<IActionResult> InsertPartsAndReplaces([FromBody] string partNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(partNumber))
        //    {
        //        return BadRequest("Номер детали не может быть пустым."); // Возвращаем 400 Bad Request
        //    }

        //    try
        //    {
        //        await _databaseService.InsertPartsAndReplacesAsync(partNumber);
        //        return Ok("Деталь успешно добавлена."); // Возвращаем 200 OK
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return StatusCode(500, $"Ошибка при вставке реплейса: {ex.Message}"); // Возвращаем 500 Internal Server Error
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Необработанная ошибка: {ex.Message}"); // Возвращаем 500 Internal Server Error
        //    }
        //}

        [HttpPut("update-status-byId/{Id}")]
        public async Task<IActionResult> UpdatePartsAndReplacesStatusById(int Id, [FromBody] int status)
        {
            await _databaseService.UpdatePartsAndReplacesStatusAsync(Id, status);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePartsAndReplaces([FromBody] PartsAndReplace item)
        {
            //await _databaseService.UpdatePartsAndReplacesAsync(item);
            //return NoContent(); // Возвращает 204 No Content

            try
            {
                
                await _databaseService.UpdatePartsAndReplacesAsync(item);
                return NoContent(); // Returns 204 No Content
            }
            catch (DbUpdateConcurrencyException ex)
            {
               return Conflict("Concurrency error: The record was modified by another user after you retrieved it.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Database error occurred while updating.  See logs for details.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. See logs for details.");
            }
        }
    }
}
