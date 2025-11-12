using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi_JobDocumentService.Services;

namespace WebApi_JobDocumentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobDocInfoController : BaseControllerClass
    {

        public JobDocInfoController(IDatabaseService databaseService) : base(databaseService)
        {
        }


        [HttpPost("insert")]
        public async Task<IActionResult> InsertJobDocsInfo([FromBody] JobDocInfo jobDocInfoDto)
        {
            if (jobDocInfoDto.JobId <= 0)
            {
                return BadRequest("Некорректный ID задания."); // Возвращаем 400 Bad Request, если ID задания некорректен
            }
            if (jobDocInfoDto.ModelsNumberId <= 0)
            {
                return BadRequest("Некорректный ID задачи."); // Возвращаем 400 Bad Request, если ID задачи некорректен
            }

            if (jobDocInfoDto.ModelId <= 0)
            {
                return BadRequest("Некорректный ID PDF."); // Возвращаем 400 Bad Request, если ID PDF некорректен
            }

            try
            {
                await _databaseService.InsertJobDocsInfoAsync(jobDocInfoDto.JobId, jobDocInfoDto.ModelsNumberId, jobDocInfoDto.ModelId);
                return CreatedAtAction(nameof(InsertJobDocsInfo), new { jobId = jobDocInfoDto.JobId }, jobDocInfoDto); // Возвращаем 201 Created
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                 return StatusCode(500, "Ошибка сервера при вставке информации о документе."); // Возвращаем 500 Internal Server Error
            }
        }


    } 
}
