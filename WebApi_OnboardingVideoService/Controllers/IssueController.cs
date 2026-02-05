using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_OnboardingVideoService.Services;

namespace WebApi_OnboardingVideoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssueController : BaseControllerClass
    {
        public IssueController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        /// <summary>
        /// Получить проблему по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Issue>> GetIssue(int id)
        {
            try
            {
                var issue = await _databaseService.GetIssueAsync(id);

                if (issue == null)
                {
                    return NotFound($"Проблема с ID {id} не найдена");
                }

                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении проблемы: {ex.Message}");
            }
        }

        /// <summary>
        /// Зарегистрировать отчет о проблеме со складом
        /// </summary>
        [HttpPost("stock")]
        public async Task<ActionResult<Issue>> RegisterStockReport([FromBody] RegisterStockRequest request)
        {
            if (request == null || request.UserId <= 0 || request.StockId <= 0)
            {
                return BadRequest("Некорректные данные запроса");
            }

            try
            {
                var issue = await _databaseService.RegisterStockReportAsync(
                    request.UserId,
                    request.StockId);

                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при регистрации отчета по складу: {ex.Message}");
            }
        }

        /// <summary>
        /// Зарегистрировать отчет о проблеме с запчастью
        /// </summary>
        [HttpPost("part")]
        public async Task<ActionResult<Issue>> RegisterPartReport([FromBody] RegisterPartRequest request)
        {
            if (request == null || request.UserId <= 0 || request.PartId <= 0)
            {
                return BadRequest("Некорректные данные запроса");
            }

            try
            {
                var issue = await _databaseService.RegisterPartReportAsync(
                    request.UserId,
                    request.PartId);

                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при регистрации отчета по запчасти: {ex.Message}");
            }
        }

        /// <summary>
        /// Зарегистрировать отчет о проблеме с похожими моделями
        /// </summary>
        [HttpPost("similar")]
        public async Task<ActionResult<Issue>> RegisterSimilarReport([FromBody] RegisterSimilarRequest request)
        {
            if (request == null || request.UserId <= 0 || request.JobId <= 0)
            {
                return BadRequest("Некорректные данные запроса");
            }

            try
            {
                var issue = await _databaseService.RegisterSimilarReportAsync(
                    request.UserId,
                    request.JobId);

                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при регистрации отчета по похожим моделям: {ex.Message}");
            }
        }

        /// <summary>
        /// Зарегистрировать отчет о проблеме с моделью
        /// </summary>
        [HttpPost("model")]
        public async Task<ActionResult<Issue>> RegisterModelReport([FromBody] RegisterModelRequest request)
        {
            if (request == null || request.UserId <= 0 || request.JobId <= 0 || request.ModelId <= 0)
            {
                return BadRequest("Некорректные данные запроса");
            }

            try
            {
                var issue = await _databaseService.RegisterModelReportAsync(
                    request.UserId,
                    request.JobId,
                    request.ModelId);

                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при регистрации отчета по модели: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавить детали к существующей проблеме
        /// </summary>
        [HttpPut("detail")]
        public async Task<ActionResult<Issue>> DetailIssue([FromBody] DetailIssueRequest request)
        {
            if (request == null || request.IssueId <= 0)
            {
                return BadRequest("Некорректные данные запроса");
            }

            try
            {
                var issue = await _databaseService.DetailIssueAsync(
                    request.IssueId,
                    request.IssueType,
                    request.IssueDetail);

                if (issue == null)
                {
                    return NotFound($"Проблема с ID {request.IssueId} не найдена");
                }

                return Ok(issue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении деталей проблемы: {ex.Message}");
            }
        }
    }

    // DTO классы для запросов
    public class RegisterStockRequest
    {
        public int UserId { get; set; }
        public int StockId { get; set; }
    }

    public class RegisterPartRequest
    {
        public int UserId { get; set; }
        public int PartId { get; set; }
    }

    public class RegisterSimilarRequest
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
    }

    public class RegisterModelRequest
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public int ModelId { get; set; }
    }

    public class DetailIssueRequest
    {
        public int IssueId { get; set; }
        public int IssueType { get; set; }
        public string? IssueDetail { get; set; }
    }
}
