using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_JobStockService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<List<JobStocksView>> GetJobStocks(int jobId);
        Task DeleteJobStocksAsync(int jobId);
        Task DeleteJobStocksByUserIdAsync(int jobId, int userStockId);
       // Task UpdateStockSearchAsync(int taskId, int status);
        Task InsertJobStocksAsync(int jobId, int userStockId, string matchPartNumber);
        Task DeleteJobStocksAsync(int jobId, int userStockId);


    }

}
