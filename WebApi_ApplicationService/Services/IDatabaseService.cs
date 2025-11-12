using System.Collections.Generic;
using System.Threading.Tasks;
using KameraData.Data.Models;

namespace WebApi_applications.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<IEnumerable<Application>> GetApplicationList();
        Task<IEnumerable<Application>> GetApplicationName(string name);
        Task UpdateApplication(Application app);

        Task<IEnumerable<KameraData.Data.Models.ApplicationLog>> GetApplicationLogList();
        Task InsertApplicationLogAsync(string applicationName, string wrnType, string wrnText, string desc = "");
        Task<bool> IsDatabaseHealthyAsync();
    }

}
