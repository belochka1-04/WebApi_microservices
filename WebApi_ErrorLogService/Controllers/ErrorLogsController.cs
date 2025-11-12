using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ErrorLogService.Services;

namespace WebApi_ErrorLogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorLogsController : BaseControllerClass
    {

        public ErrorLogsController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("getall")]
        public async Task<ActionResult<List<ErrorLog>>> GetAllErrorLogs()
        {
            try
            {
                var errorLogs = await _databaseService.GetErrorLogAsync();
                return Ok(errorLogs);
            }
            catch
            {
                return StatusCode(500, "Internal server error"); // Or a more descriptive message
            }
        }


        [HttpGet("get/{id}")]
        public async Task<ActionResult<ErrorLog>> GetErrorLogById(int id) // Changed parameter name to 'id' to match route
        {
            try
            {
                var errorLog = await _databaseService.GetErrorLogByIdAsync(id);

                if (errorLog == null)
                {
                    return NotFound($"ErrorLog with id {id} not found.");
                }

                return Ok(errorLog);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("insert")]
        public async Task<ActionResult<ErrorLog>> InsertErrorLogAsync([FromBody] ErrorLog request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                var insertedErrorLog = await _databaseService.InsertErrorLogAsync(request); // Assuming you return the inserted object
                return CreatedAtAction(nameof(GetErrorLogById), new { id = insertedErrorLog.id }, insertedErrorLog);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateErrorLogAsync(int id, [FromBody] ErrorLog request)
        {
            if (request == null || id != request.id)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                var existingErrorLog = await _databaseService.GetErrorLogByIdAsync(id);
                if (existingErrorLog == null)
                {
                    return NotFound($"ErrorLog with id {id} not found.");
                }

                await _databaseService.UpdateErrorLogAsync(request);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteErrorLogAsync(int id)  // Changed parameter name to 'id' to match route
        {
            try
            {
                var existingErrorLog = await _databaseService.GetErrorLogByIdAsync(id);
                if (existingErrorLog == null)
                {
                    return NotFound($"ErrorLog with id {id} not found.");
                }

                await _databaseService.DeleteErrorLogAsync(id);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
