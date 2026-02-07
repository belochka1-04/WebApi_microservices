using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_UserService.Services;

namespace WebApi_UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _databaseService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("getuserbyTg/{Id}")]
        public async Task<ActionResult<User>> GetUserByTgId(int Id)
        {
            var job = await _databaseService.GetUserByTgAsync(Id);
            return Ok(job);
        }

        [HttpPost("create-or-get")]
        public async Task<ActionResult<User>> CreateOrGetUser([FromBody] CreateOrGetUserRequest request)
        {
            if (request == null || request.TelegramId == 0)
                return BadRequest("TelegramId is required.");

            try
            {
                var user = await _databaseService.CreateOrGetUserAsync(
                    request.TelegramId,
                    request.ReferralUserId
                );

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании/получении пользователя: {ex.Message}");
            }
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
        [HttpPost("{id:int}/tips/video-seen")]
        public async Task<IActionResult> UpdateLastVideoTip(int id, [FromBody] UpdateTipRequest request)
        {
            if (request == null)
                return BadRequest("TipId is required.");

            var ok = await _databaseService.UpdateLastVideoTipAsync(id, request.TipId);
            if (!ok)
                return NotFound();

            return Ok();
        }

        [HttpPost("{id:int}/tips/link-seen")]
        public async Task<IActionResult> UpdateLastLinkTip(int id, [FromBody] UpdateTipRequest request)
        {
            if (request == null)
                return BadRequest("TipId is required.");

            var ok = await _databaseService.UpdateLastLinkTipAsync(id, request.TipId);
            if (!ok)
                return NotFound();

            return Ok();
        }

        [HttpPost("{id:int}/onboarding/repair-video")]
        public async Task<IActionResult> UpdateLastRepairVideo(int id, [FromBody] UpdateOnboardingVideoRequest request)
        {
            if (request == null)
                return BadRequest("VideoId is required.");

            var ok = await _databaseService.UpdateLastRepairVideoAsync(id, request.VideoId);
            if (!ok)
                return NotFound();

            return Ok();
        }

        [HttpPost("{id:int}/onboarding/warehouse-video")]
        public async Task<IActionResult> UpdateLastWarehouseVideo(int id, [FromBody] UpdateOnboardingVideoRequest request)
        {
            if (request == null)
                return BadRequest("VideoId is required.");

            var ok = await _databaseService.UpdateLastWarehouseVideoAsync(id, request.VideoId);
            if (!ok)
                return NotFound();

            return Ok();
        }


        [HttpPost("{id:int}/mode")]
        public async Task<IActionResult> UpdateMode(int id, [FromBody] UpdateUserModeRequest request)
        {
            if (request == null)
                return BadRequest("Mode is required.");

            try
            {
                var updated = await _databaseService.UpdateUserModeAsync(id, request.Mode);
                if (!updated)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при смене режима пользователя: {ex.Message}");
            }
        }

        [HttpPost("{id:int}/plan/extend-pro")]
public async Task<IActionResult> ExtendPro(int id, [FromBody] ExtendProRequest request)
{
    if (request == null || request.Months <= 0)
        return BadRequest("Months must be positive.");

    try
    {
        var success = await _databaseService.ExtendProAsync(id, request.Months);
        if (!success)
            return NotFound();

        return Ok();
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Ошибка при продлении Pro: {ex.Message}");
    }
}

    }

    public class CreateOrGetUserRequest
    {
        public long TelegramId { get; set; }
        public int? ReferralUserId { get; set; } // пока не используем, но поле оставим
    }
}
