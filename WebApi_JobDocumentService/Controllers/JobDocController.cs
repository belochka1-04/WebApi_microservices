
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_JobDocumentService.Services;

namespace WebApi_JobDocumentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobDocController : BaseControllerClass
    {
        public JobDocController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        // ========================================
        // ИСПРАВЛЕННЫЕ СУЩЕСТВУЮЩИЕ МЕТОДЫ
        // ========================================

        /// <summary>
        /// Получить главный документ по jobId
        /// </summary>
        [HttpGet("getMainDoc/{jobId}")]
        [ProducesResponseType(typeof(JobDoc), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JobDoc>> GetMainDoc(int jobId)
        {
            try
            {
                var job = await _databaseService.GetMainDoc(jobId);

                // Проверка на null или пустой объект
                if (job == null || job.Id == 0)
                {
                    return NotFound($"Main document not found for job {jobId}");
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить PDF документ по jobId
        /// </summary>
        [HttpGet("getPdfDoc/{jobId}")]
        [ProducesResponseType(typeof(JobDoc), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JobDoc>> GetPdfDoc(int jobId)
        {
            try
            {
                var job = await _databaseService.GetPdfDoc(jobId);

                // Проверка на null или пустой объект
                if (job == null || job.Id == 0)
                {
                    return NotFound($"PDF document not found for job {jobId}");
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Вставить простой JobDoc (только JobId и ModelsId)
        /// </summary>
        [HttpPost("insert")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertJobDoc([FromBody] JobDoc request)
        {
            if (request == null || request.JobId <= 0 || request.ModelsId <= 0)
            {
                return BadRequest("Invalid request: JobId and ModelsId are required");
            }

            try
            {
                await _databaseService.InsertJobDocsAsync(request.JobId.Value, request.ModelsId);
                return Ok("JobDoc successfully inserted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Вставить полный JobDoc со всеми полями
        /// </summary>
        [HttpPost("insertFull")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertFullJobDoc([FromBody] JobDoc request)
        {
            if (request == null || request.JobId <= 0 || request.ModelsId <= 0)
            {
                return BadRequest("Invalid request: JobId and ModelsId are required");
            }

            try
            {
                await _databaseService.InsertFullJobDocsAsync(request);
                return Ok("Full JobDoc successfully inserted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Вставить несколько JobDoc пакетно
        /// </summary>
        [HttpPost("insertFullBatch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertFullJobDocsBatch([FromBody] List<JobDoc> docs)
        {
            if (docs == null || !docs.Any())
            {
                return BadRequest("Invalid request: Empty document list");
            }

            try
            {
                await _databaseService.InsertFullJobDocsBatchAsync(docs);
                return Ok($"Successfully inserted {docs.Count} documents");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить JobDoc по jobId и modelId
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteJobDoc([FromQuery] int jobId, [FromQuery] int modelId)
        {
            if (jobId <= 0 || modelId <= 0)
            {
                return BadRequest("Invalid request: JobId and ModelId must be greater than 0");
            }

            try
            {
                await _databaseService.DeleteJobDocsAsync(jobId, modelId);
                return Ok("JobDoc successfully deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить все JobDocs по jobId
        /// </summary>
        [HttpDelete("deleteByJob/{jobId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteJobDocByJobId(int jobId)
        {
            if (jobId <= 0)
            {
                return BadRequest("Invalid request: JobId must be greater than 0");
            }

            try
            {
                await _databaseService.DeleteJobDocsByJobIdAsync(jobId);
                return Ok($"All JobDocs for job {jobId} successfully deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ========================================
        // НОВЫЕ МЕТОДЫ (ДОБАВИТЬ!)
        // ========================================

        /// <summary>
        /// Получить ВСЕ документы по jobId
        /// GET /api/JobDoc/all-by-job/{jobId}
        /// </summary>
        [HttpGet("all-by-job/{jobId}")]
        [ProducesResponseType(typeof(List<JobDoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<JobDoc>>> GetAllByJob(int jobId)
        {
            try
            {
                var docs = await _databaseService.GetAllJobDocsByJobIdAsync(jobId);

                if (docs == null || docs.Count == 0)
                {
                    return NotFound($"No documents found for job {jobId}");
                }

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить документы по jobId с фильтрами
        /// GET /api/JobDoc/by-job/{jobId}?siteState=1&docState=1&documentType=Diagram PDF&cleanedModel=ABC123
        /// </summary>
        [HttpGet("by-job/{jobId}")]
        [ProducesResponseType(typeof(List<JobDoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<JobDoc>>> GetByJobWithFilters(
            int jobId,
            [FromQuery] int? siteState = null,
            [FromQuery] int? docState = null,
            [FromQuery] int? partCountState = null,
            [FromQuery] string? documentType = null,
            [FromQuery] string? cleanedModel = null)
        {
            try
            {
                var docs = await _databaseService.GetJobDocsByJobIdWithFiltersAsync(
                    jobId, siteState, docState, partCountState, documentType, cleanedModel);

                if (docs == null || docs.Count == 0)
                {
                    return NotFound($"No documents found for job {jobId} with specified filters");
                }

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить документы по списку ID
        /// GET /api/JobDoc/by-ids?ids=1&ids=2&ids=3
        /// </summary>
        [HttpGet("by-ids")]
        [ProducesResponseType(typeof(List<JobDoc>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<JobDoc>>> GetByIds([FromQuery] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("Invalid request: IDs list cannot be empty");
            }

            try
            {
                var docs = await _databaseService.GetJobDocsByIdsAsync(ids);

                if (docs == null || docs.Count == 0)
                {
                    return NotFound("No documents found for specified IDs");
                }

                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}