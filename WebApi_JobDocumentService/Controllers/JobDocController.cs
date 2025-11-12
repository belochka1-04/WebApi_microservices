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


        [HttpGet("getMainDoc/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobDoc>>> GetMainDoc(int jobId)
        {
            var jobs = await _databaseService.GetMainDoc(jobId);
            return Ok(jobs);
        }

        [HttpGet("getPdfDoc/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobDoc>>> GetPdfDoc(int jobId)
        {
            var jobs = await _databaseService.GetPdfDoc(jobId);
            return Ok(jobs);
        }
        


        [HttpPost("insert")]
        public async Task<IActionResult> InsertJobDoc([FromBody] JobDoc request)
        {
            if (request == null || request.JobId <= 0 || request.ModelsId <= 0)
            {
                return BadRequest("                   ."); //            400 Bad Request,                        
            }

            try
            {
                //                                           
                await _databaseService.InsertJobDocsAsync(request.JobId.Value, request.ModelsId);
                return Ok("                                 ."); //            200 OK             
            }
            catch (Exception ex)
            {
                //                
                return StatusCode(500, "                                            ."); //            500 Internal Server Error
            }
        }

        [HttpPost("insertFull")]
        public async Task<IActionResult> InsertFullJobDoc([FromBody] JobDoc request)
        {
            if (request == null || request.JobId <= 0 || request.ModelsId <= 0)
            {
                return BadRequest("                   ."); //            400 Bad Request,                        
            }

            try
            {
                //                                           
                await _databaseService.InsertFullJobDocsAsync(request);
                return Ok("                                 ."); //            200 OK             
            }
            catch (Exception ex)
            {
                //                
                return StatusCode(500, "                                            ."); //            500 Internal Server Error
            }
        }

        [HttpPost("insertFullBatch")]
        public async Task<IActionResult> InsertFullJobDocsBatch([FromBody] List<JobDoc> docs)
        {
            if (docs == null || !docs.Any())
            {
                return BadRequest("Invalid request: Empty document list.");
            }

            try
            {
                await _databaseService.InsertFullJobDocsBatchAsync(docs);
              

                return Ok($"Successfully inserted {docs.Count} documents.");
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteJobDoc([FromQuery] int jobId, [FromQuery] int modelId)
        {
            if (jobId <= 0 || modelId <= 0)
            {
                return BadRequest("                   ."); //            400 Bad Request,                        
            }

            try
            {
                await _databaseService.DeleteJobDocsAsync(jobId, modelId);
                return Ok("                               ."); //            200 OK             
            }
            catch (Exception ex)
            {
                //                
                return StatusCode(500, "                                             ."); //            500 Internal Server Error
            }
        }


        
        [HttpDelete("deleteByJob/{jobId}")]
        public async Task<IActionResult> DeleteJobDocByJobId(int jobId)
        {
            if (jobId <= 0)
            {
                return BadRequest("                   ."); //            400 Bad Request,                        
            }

            try
            {
                await _databaseService.DeleteJobDocsByJobIdAsync(jobId);
                return Ok("                               ."); //            200 OK             
            }
            catch (Exception ex)
            {
                //                
                return StatusCode(500, "                                             ."); //            500 Internal Server Error
            }
        }
    
    } 
}
