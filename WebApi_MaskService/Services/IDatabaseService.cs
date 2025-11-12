using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_MaskService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<IEnumerable<Mask>> GetAllMasksAsync(int jobID);

    }

}
