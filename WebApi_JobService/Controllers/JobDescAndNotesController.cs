
using JobService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobDescAndNotesController : BaseControllerClass
    {
        public JobDescAndNotesController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        // Метод для получения всех заданий
        [HttpGet("jobs")]
        public async Task<ActionResult<IEnumerable<JobDescriptionAndNote>>> GetJobs()
        {
            var jobs = await _databaseService.GetJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("jobsForJob/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobDescriptionAndNote>>> GetJobsForJobId(int jobId)
        {
            var jobs = await _databaseService.GetJandD(jobId);
            return Ok(jobs);
        }

        // Метод для обновления статуса описания и заметок
        [HttpPut("update-status/{jobId}")]
        public async Task<IActionResult> UpdateJobStatus(int jobId, [FromBody] int status)
        {
            await _databaseService.UpdateDescriptionAndNotesStatusAsync(jobId, status);
            return NoContent();
        }

        // Метод для обновления описания
        [HttpPut("update-descr/{jobId}")]
        public async Task<IActionResult> UpdateJobDescription(int jobId, [FromBody] string text)
        {
            await _databaseService.UpdateDescriptionAndNotesDescriptionAsync(jobId, text);
            return NoContent();
        }

        // Метод для обновления pic_count
        [HttpPut("update-piccount/{jobId}")]
        public async Task<IActionResult> UpdateJobDescriptionPicCount(int jobId, [FromBody] int pic_count)
        {
            await _databaseService.UpdateDescriptionAndNotesPicCountAsync(jobId, pic_count);
            return NoContent();
        }

        [HttpPost("insert-descr/{jobId}")]
        public async Task<IActionResult> InsertJobDescription(int jobId, [FromBody] string text)
        {
            await _databaseService.InsertDescriptionAndNotesDescriptionAsync(jobId, text);
            return NoContent();
        }

        /// <summary>
        /// Пометить записи job_description_and_notes с указанным текстом как google_confirmed = 1.
        /// POST /api/JobDescAndNotes/confirm-google
        /// </summary>
        /// <param name="requestWord">
        /// Полный текст (job_description + job_notes), по которому нужно подтвердить записи.
        /// Сравнение выполняется без учёта регистра и лишних пробелов.
        /// </param>
        [HttpPost("confirm-google")]
        public async Task<IActionResult> ConfirmGoogle([FromBody] string requestWord)
        {
            if (string.IsNullOrWhiteSpace(requestWord))
                return BadRequest("requestWord is required.");

            await _databaseService.MarkJobDescriptionsGoogleConfirmedAsync(requestWord);
            return Ok();
        }
    }
}
