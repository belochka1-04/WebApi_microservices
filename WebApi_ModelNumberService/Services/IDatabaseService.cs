using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_ModelNumberService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<List<ModelNumber>> GetWasteTasksAsync();
        Task<ModelNumber> GetNextTaskAsync();

        Task<List<ModelNumber>> GetNextTasksWithConfAsync(string Conf);
        Task<ModelNumber> GetNextTaskWithConfAsync(string Conf);
        Task InsertModelNumberAsync(int jobId, KameraData.Data.Models.Model model, string search_text, int status, int count);
        Task InsertModelNumberBatchAsync(List<ModelNumberRequest> requests);
        Task InsertModelNumberBatchAsync(int jobId, KameraData.Data.Models.Model model, string search_text, int status, int count);
        Task InsertNModelNumberAsync(int jobId, KameraData.Data.Models.NModel model, string search_text, int status, int count);
        Task DeleteModelNumbersAsync(int jobId);
        Task UpdateOtherModelNumbersStatusAsync(int jobId, int status);
        Task UpdateTaskStatusAsync(int taskId, int status);
        Task InsertModelNumberNotFoundAsync(int jobId, string modelNumber);
        Task DeleteModelNumbersNotFoundAsync(int jobId);


    }

}
