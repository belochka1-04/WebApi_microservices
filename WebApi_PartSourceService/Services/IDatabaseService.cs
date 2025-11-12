using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_PartSourceService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<PartSource> GetPartSourceById(int id);
        Task<PartSource> GetPartSourceByUrl(string url);
        Task<List<PartSource>> GetPartSource();
        Task<bool> DeletePartSource(int id);
        Task<bool> UpdatePartSource(PartSource partSource);
        Task<bool> AddPartSource(PartSource partSource);

    }

}
