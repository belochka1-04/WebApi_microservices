using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi_ImageJobService.Services;

namespace WebApi_ImageJobService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageTextController :  BaseControllerClass
    {

        public ImageTextController(IDatabaseService databaseService) : base(databaseService)
        {
        }

        // Метод для обновления статуса описания и заметок
        [HttpPut("AddText")]
        public async Task<IActionResult> UpdateJobStatus( [FromBody] ImageText text)
        {
            await _databaseService.SaveTextToBDAsync(text.text, text.folder, text.file);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPut("AddJobPic")]
        public async Task<IActionResult> AddJobPic([FromBody] JobPic pic)
        {
            await _databaseService.InsertImageFile(pic.FolderName, pic.FileName, pic.OcrText, pic.IsRating, pic.CompletionCosts,pic.ImageToken,pic.Brand, pic.Model,pic.SerialNumber);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpPut("UpdateJobPic")]
        public async Task<IActionResult> UpdateJobPic([FromBody] JobPic pic)
        {
            await _databaseService.UpdateJobPic(pic);
            return NoContent(); // Возвращает 204 No Content
        }

        [HttpGet("GetJobPicList/{fileName}")]
        public async Task<ActionResult<IEnumerable<JobDescriptionAndNote>>> GetJobPicList(string fileName)
        {
            var jobs = await _databaseService.GetJobPicList(fileName);
            return Ok(jobs);
        }
        
    }
}
