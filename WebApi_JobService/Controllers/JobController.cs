using KameraData.Data.Dtos;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi_JobService.Services;

namespace WebApi_JobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        // ---------- НОВЫЕ МЕТОДЫ ----------

        // GET api/job/today-count/123
        [HttpGet("today-count/{userId:int}")]
        public async Task<ActionResult<int>> GetTodaysOperationsCount(int userId)
        {
            if (userId <= 0)
                return BadRequest("userId должен быть > 0");

            var count = await _databaseService.GetTodaysOperationsCountAsync(userId);
            return Ok(count);
        }

        // POST api/job/create-from-text
        [HttpPost("create-from-text")]
        public async Task<ActionResult<Job>> CreateFromText([FromBody] CreateJobFromTextRequest request)
        {
            if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("UserId и Content обязательны");
            }

            try
            {
                var job = await _databaseService.CreateJobFromTextAsync(
                    request.UserId,
                    request.Content,
                    request.Brand);

                return CreatedAtAction(nameof(GetFullJobById), new { jobId = job.Id }, job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании задания из текста: {ex.Message}");
            }
        }

        // POST api/job/create-from-image
        [HttpPost("create-from-image")]
        public async Task<ActionResult<Job>> CreateFromImage([FromBody] CreateJobFromImageDto dto)
        {
            if (dto.UserId <= 0 || string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                return BadRequest("UserId и ImageUrl обязательны");
            }

            try
            {
                var job = await _databaseService.CreateJobFromImageAsync(dto);
                return CreatedAtAction(nameof(GetFullJobById), new { jobId = job.Id }, job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при создании задания из изображения: {ex.Message}");
            }
        }

        // GET api/job/getfulljob/10
        [HttpGet("getfulljob/{jobId:int}")]
        public async Task<ActionResult<Job>> GetFullJobById(int jobId)
        {
            var job = await _databaseService.GetFullJobByIdAsync(jobId);
            if (job == null)
                return NotFound();

            return Ok(job);
        }
    }

    // DTO для create-from-text
    public class CreateJobFromTextRequest
    {
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public string? Brand { get; set; }
    }
}

