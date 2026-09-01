using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_StockCredsService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task UpdateStockCreds(StockCred stockCred);
        Task<List<StockCred>> GetStockCreds();
        Task<List<StockCred>> ClaimDueStockCreds(string workerId, int batchSize, int leaseSeconds);
        Task ReleaseStockCredLease(int stockCredId, string workerId);


    }

}
