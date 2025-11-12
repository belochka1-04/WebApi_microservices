using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_PartsService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<List<PartsAndReplace>> GetPartsAndReplacesAsync(List<int> replaceIds);
        Task<List<PartsAndReplace>> GetPartsAndReplacesWithStateAsync(string state);
        Task<List<PartsAndReplace>> GetPartsAndReplacesByUserIDAsync(int userID);
        Task InsertPartsAndReplacesAsync(string partNumber);
        Task UpdatePartsAndReplacesStatusAsync(int Id, int status);
        Task UpdatePartsAndReplacesAsync(PartsAndReplace item);
        Task<PartsNamesArchive> GetPartsNamesArchiveById(int id);
        Task<bool> DeletePartsNamesArchive(int id);
        Task<bool> UpdatePartsNamesArchive(PartsNamesArchive partsNamesArchive);
        Task<bool> AddPartsNamesArchive(PartsNamesArchive partsNamesArchive);
        Task<List<Parts>> GetPartsByModelIdAsync(int modelId);


    }

}
