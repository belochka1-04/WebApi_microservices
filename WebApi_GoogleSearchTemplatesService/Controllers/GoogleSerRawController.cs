using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_GoogleSearchTemplatesService.Services;

namespace WebApi_GoogleSearchTemplatesService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class GoogleSerRawController : BaseControllerClass
	{
		private readonly ILogger<GoogleSerRawController> _logger;

		public GoogleSerRawController(IDatabaseService databaseService, ILogger<GoogleSerRawController> logger)
			: base(databaseService)
		{
            _logger = logger;
		}

		/// <summary>
		/// Сохранить одну запись SERP.
		/// POST /api/GoogleSerRaw
		/// </summary>
		/// <param name="serp">Данные по одному результату SERP.</param>
		/// <returns>Id созданной записи.</returns>
		[HttpPost]
		[ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> Create([FromBody] GoogleSerpRaw serp)
		{
			if (serp == null)
				return BadRequest("Body is required.");

			try
			{
				var id = await _databaseService.InsertGoogleSerpRawAsync(serp);
				return CreatedAtAction(nameof(Create), new { id }, id);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ошибка сохранения SERP: {ex.Message}");
			}
		}

		/// <summary>
		/// Сохранить батч SERP-записей.
		/// POST /api/GoogleSerRaw/batch
		/// </summary>
		/// <param name="serps">Коллекция результатов SERP.</param>
		/// <returns>Список Id созданных записей.</returns>
		[HttpPost("batch")]
		[ProducesResponseType(typeof(IReadOnlyList<int>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<int>>> CreateBatch([FromBody] IEnumerable<GoogleSerpRaw> serps)
        {
            var traceId = HttpContext.TraceIdentifier;
            Response.Headers["X-Trace-Id"] = traceId;
            if (serps == null)
                return BadRequest("Body is required.");
            try
            {
                var ids = await _databaseService.InsertGoogleSerpRawBatchAsync(serps);
                return Ok(ids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST /api/GoogleSerRaw/batch failed: TraceId={TraceId}", traceId);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Failed to save SERP batch.",
                    Detail = "See API logs using the traceId.",
                    Instance = HttpContext.Request.Path.Value
                };
                problem.Extensions["traceId"] = traceId;
                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
    }
}
