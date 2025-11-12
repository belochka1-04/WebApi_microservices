using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_ModelService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {

        Task<List<KameraData.Data.Models.Model>> FindExactMatchInModelsAsync(string text);
        Task<List<KameraData.Data.Models.Model>> GetAllModelsAsync();
        Task<List<KameraData.Data.Models.NModel>> GetAllNModelsAsync();
        Task<KameraData.Data.Models.Model> GetModelById(int Id);
        Task<KameraData.Data.Models.NModel> GetNModelById(int Id);
        Task<KameraData.Data.Models.Model> GetModelById2(int Id);
        Task<KameraData.Data.Models.NModel> GetNModelById2(int Id);
        Task<List<KameraData.Data.Models.Model>> FindSubstringMatchInModelsAsync(string modelNumber);
        Task<List<KameraData.Data.Models.NModel>> FindSubstringMatchInNModelsAsync(string modelNumber);
        Task<List<KameraData.Data.Models.Model>> GetModelsWithNumAsync(string modelNumber);
        Task<List<KameraData.Data.Models.Model>> GetModelsWithNum3Async(string modelNumber);
        Task<List<KameraData.Data.Models.JobDoc>> GetJDModelsWithNumAsync(string modelNumber);
        Task GetJobDocModelsLikeNameAndInsertAsync(string modelNumber, int jobId, int maxLen, int minLen);
        Task<List<string>> GetModelsLevaAsync(string InputTitle, string? InputBrandCode);
        Task<List<KameraData.Data.Models.NModel>> GetNModelsWithNumAsync(string modelNumber);
        Task<List<KameraData.Data.Models.Model>> GetModelsWithNumForTrimAsync(string modelNumber);
        Task<List<KameraData.Data.Models.Model>> GetModelsWithNumForTrim3Async(string modelNumber);
        Task<List<KameraData.Data.Models.NModel>> GetNModelsWithNumForTrimAsync(string modelNumber);

    }

}
