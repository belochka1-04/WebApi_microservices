using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_ResponsesService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task InsertResponceAsync(KameraData.Data.Models.Response responce);
        Task<List<KameraData.Data.Models.Response>> GetResponceByJobIdAsync(int jobId);

    }

}
