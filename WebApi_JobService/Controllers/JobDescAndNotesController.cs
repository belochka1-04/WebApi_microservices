using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return NoContent(); // Возвращает 204 No Content
        }

        // Метод для обновления статуса описания и заметок
        [HttpPut("update-descr/{jobId}")]
        public async Task<IActionResult> UpdateJobDescription(int jobId, [FromBody] string text)
        {
            await _databaseService.UpdateDescriptionAndNotesDescriptionAsync(jobId, text);
            return NoContent(); // Возвращает 204 No Content
        }

        // Метод для обновления статуса описания и заметок
        [HttpPut("update-piccount/{jobId}")]
        public async Task<IActionResult> UpdateJobDescriptionPicCount(int jobId, [FromBody] int pic_count)
        {
            await _databaseService.UpdateDescriptionAndNotesPicCountAsync(jobId, pic_count);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPost("insert-descr/{jobId}")]
        public async Task<IActionResult> InsertJobDescription(int jobId, [FromBody] string text)
        {
            await _databaseService.InsertDescriptionAndNotesDescriptionAsync(jobId, text);
            return NoContent(); // Возвращает 204 No Content
        }
    }
}
