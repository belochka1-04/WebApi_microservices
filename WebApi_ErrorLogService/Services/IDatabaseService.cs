using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_ErrorLogService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task UpdateErrorLogAsync(ErrorLog errorLog);
        Task DeleteErrorLogAsync(int id);
        Task<ErrorLog> InsertErrorLogAsync(ErrorLog errorLog);
        Task<ErrorLog> GetErrorLogByIdAsync(int id);
        Task<List<ErrorLog>> GetErrorLogAsync();


    }

}
