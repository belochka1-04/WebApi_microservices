using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_PricebotTasksService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<PricebotTask> GetPricebotTaskByIdAsync(int id);
        Task<List<PricebotTask>> GetPricebotTaskByIdByZeroStateAsync();

        Task CreatePricebotTaskAsync(PricebotTask task);
        Task UpdatePricebotTaskAsync(PricebotTask task);
        Task DeletePricebotTaskAsync(int id);
        Task<bool> PricebotTaskExistsAsync(int id);
    }

}
