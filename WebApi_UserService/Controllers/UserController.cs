using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_UserService.Services;

namespace WebApi_UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseControllerClass
    {

        public UserController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("getuser/{userId}")]
        public async Task<ActionResult<int>> GetUserCrmById(int userId)
        {
            var job = await _databaseService.GetUserCrmAsync(userId);
            return Ok(job);
        }

        [HttpGet("getuserbyTg/{Id}")]
        public async Task<ActionResult<User>> GetUserByTgId(int Id)
        {
            var job = await _databaseService.GetUserByTgAsync(Id);
            return Ok(job);
        }

        // Метод для сохранения нового пользователя
        [HttpPost("save")]
        public async Task<IActionResult> SaveUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Пользователь не может быть null");
            }

            try
            {
                await _databaseService.SaveUserToBDAsync(user);
                return Ok("Пользователь успешно сохранен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при сохранении пользователя: {ex.Message}");
            }
        }

        // Метод для сохранения нового пользователя
        [HttpPost("saveDefaultUser")]
        public async Task<IActionResult> SaveDefaultUser([FromBody] SaveDefaultUserRequest request)
        {
            if (request.TelegramId == null)
            {
                return BadRequest("Telegram ID не может быть null");
            }

            try
            {
                await _databaseService.SaveDefaultUserToBDAsync(request.TelegramId, request.PrefersTelegram);
                return Ok("Пользователь успешно сохранен");
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Telegram ID "))
                    return StatusCode(400, $"Ошибка при сохранении пользователя: {ex.Message}");
                else
                    return StatusCode(500, $"Ошибка при сохранении пользователя: {ex.Message}");
            }
        }


        public class SaveDefaultUserRequest
        {
            public int? TelegramId { get; set; }
            public string? PrefersTelegram { get; set; }
        }

    [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user == null || user.Id <= 0)
            {
                return BadRequest("Некорректные данные пользователя");
            }

            try
            {
                await _databaseService.UpdateUserInBDAsync(user);
                return Ok("Пользователь успешно обновлен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении пользователя: {ex.Message}");
            }
        }
    }


}
