using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_OnboardingVideoService.Services;

namespace WebApi_OnboardingVideoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipsController : BaseControllerClass
    {
        public TipsController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        /// <summary>
        /// Получить подсказки для пользователя
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<TipsResponse>> GetTipsForUser(int userId, [FromQuery] int? lastVideoTipId, [FromQuery] int? lastLinkTipId)
        {
            try
            {
                var result = await _databaseService.GetTipsForUserAsync(userId, lastVideoTipId, lastLinkTipId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении подсказок: {ex.Message}");
            }
        }

        /// <summary>
        /// Зарегистрировать показ подсказок (уменьшить счетчики)
        /// </summary>
        [HttpPost("register-show")]
        public async Task<ActionResult> RegisterShow()
        {
            try
            {
                await _databaseService.RegisterTipsShowAsync();
                return Ok("Показы зарегистрированы");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при регистрации показа: {ex.Message}");
            }
        }
    }
}
