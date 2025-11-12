using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_ReplacesArchiveService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<ReplacesArchive> GetReplacesArchiveById(int id);
        Task<bool> DeleteReplacesArchive(int id);
        Task<bool> UpdateReplacesArchive(ReplacesArchive replacesArchive);
        Task<bool> AddReplacesArchive(ReplacesArchive replacesArchive);

    }

}
