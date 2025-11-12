using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using WebApi_PricebotTasksService.Services;

namespace WebApi_PricebotTasksService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricebotTasksController : BaseControllerClass
    {
        
        public PricebotTasksController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        [HttpGet("get/{Id}")]
        public async Task<ActionResult<Document>> GetPricebotTask(int Id)
        {
            try
            {
                var pricebotTask = await _databaseService.GetPricebotTaskByIdAsync(Id);

                if (pricebotTask == null)
                {
                    return NotFound(); // Return 404 Not Found
                }

                return Ok(pricebotTask); // Return 200 OK with the task
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 Internal Server Error
            }
        }

        [HttpGet("getByZeroState")]
        public async Task<ActionResult<List<Document>>> GetPricebotByZeroStateTask()
        {
            try
            {
                var pricebotTask = await _databaseService.GetPricebotTaskByIdByZeroStateAsync();

                if (pricebotTask == null)
                {
                    return NotFound(); // Return 404 Not Found
                }

                return Ok(pricebotTask); // Return 200 OK with the task
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 Internal Server Error
            }
        }

        // PUT: api/PricebotTasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricebotTask(int id, PricebotTask pricebotTask)
        {
            if (id != pricebotTask.Id)
            {
                return BadRequest(); // Return 400 Bad Request
            }

            try
            {
                await _databaseService.UpdatePricebotTaskAsync(pricebotTask);
                return NoContent(); // Return 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _databaseService.PricebotTaskExistsAsync(id) == false)
                {
                    return NotFound(); // Return 404 Not Found
                }
                else
                {
                    return StatusCode(500, "Concurrency error updating pricebot task."); // Return 500
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 Internal Server Error
            }
        }


        [HttpPost]
        public async Task<ActionResult<PricebotTask>> PostPricebotTask(PricebotTask pricebotTask)
        {
            try
            {
                await _databaseService.CreatePricebotTaskAsync(pricebotTask);
                return CreatedAtAction("GetPricebotTask", new { id = pricebotTask.Id }, pricebotTask); // Return 201 Created
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 Internal Server Error
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricebotTask(int id)
        {
            try
            {
                var task = await _databaseService.GetPricebotTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(); // Return 404 Not Found
                }

                await _databaseService.DeletePricebotTaskAsync(id);
                return NoContent(); // Return 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); // Return 500 Internal Server Error
            }
        }



    }
}
