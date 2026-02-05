using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_OnboardingVideoService.Services;

namespace WebApi_OnboardingVideoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnboardingVideoController : BaseControllerClass
    {
        public OnboardingVideoController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        /// <summary>
        /// Получить следующее видео для ветки "repair" (модели)
        /// </summary>
        /// <param name="lastId">ID последнего просмотренного видео (опционально)</param>
        [HttpGet("models")]
        public async Task<ActionResult<OnboardingVideo>> GetModelsAsync([FromQuery] int? lastId)
        {
            try
            {
                var result = await _databaseService.GetModelsAsync(lastId);

                if (result == null)
                {
                    return NotFound("Видео не найдено или все видео уже просмотрены");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении видео моделей: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить следующее видео для ветки "warehouse" (запчасти)
        /// </summary>
        /// <param name="lastId">ID последнего просмотренного видео (опционально)</param>
        [HttpGet("parts")]
        public async Task<ActionResult<OnboardingVideo>> GetPartsAsync([FromQuery] int? lastId)
        {
            try
            {
                var result = await _databaseService.GetPartsAsync(lastId);

                if (result == null)
                {
                    return NotFound("Видео не найдено или все видео уже просмотрены");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении видео запчастей: {ex.Message}");
            }
        }
    }
}
