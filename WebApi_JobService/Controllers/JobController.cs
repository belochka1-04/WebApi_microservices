using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : BaseControllerClass
    {
        
        public JobController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("getdtojob/{id:int}")]
        public async Task<ActionResult<JobDto>> GetById(int id)
        {
            var job = await _databaseService.GetJobDtoByIdAsync(id);
            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpGet("getjob/{jobId}")]
        public async Task<ActionResult<Job>> GetJobById(int jobId)
        {
            var job = await _databaseService.GetJobByIdAsync(jobId);
            return Ok(job);
        }

        [HttpGet("getjobByLink/{jobLink}")]
        public async Task<ActionResult<IEnumerable<JobDescriptionAndNote>>> GetJobsForJobId(string jobLink)
        {
            var jobs = await _databaseService.GetJobByLink(jobLink);
            return Ok(jobs);
        }

        [HttpPost("addJobByTg")]
        public async Task<IActionResult> AddJobByTg([FromBody] int Id)
        {
            if ( Id <= 0)
            {
                return BadRequest("Некорректные данные для задания");
            }

            try
            {
                var newJob = await _databaseService.AddJobByTgAsync(Id);
                return CreatedAtAction(nameof(GetJobById), new { jobId = newJob.Id }, newJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении задания: {ex.Message}");
            }
        }
    }
}
