using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_ImageJobService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {

		Task SaveTextToBDAsync(string text, string folderName, string fileName);
		Task InsertImageFile(string folder_name, string file_name, string ocr_text, int is_rating_plate_percentage, decimal completion_cost, int image_tokens = 0, string brand = "NULL", string model = "NULL", string serial_number = "NULL");
		Task UpdateJobPic(JobPic pic);
		Task<IEnumerable<JobPic>> GetJobPicList(string fileName);

	}

}
