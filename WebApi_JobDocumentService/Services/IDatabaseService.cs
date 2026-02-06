using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_JobDocumentService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<List<JobDocsModelInfo>> GetJobDocModelInfos(int jobId);
        Task InsertJobDocsAsync(int jobId, int modelsId);
        Task InsertFullJobDocsAsync(JobDoc job);
        Task InsertFullJobDocsBatchAsync(List<JobDoc> docs);
        Task DeleteJobDocsAsync(int jobId, int modelId);
        Task DeleteJobDocsByJobIdAsync(int jobId);
        Task<JobDoc> GetMainDoc(int jobID);
        Task<JobDoc> GetPdfDoc(int jobID);
        Task InsertJobDocsInfoAsync(int jobId, int taskId, int gotPartsListPdfId);
        /// <summary>
        /// Получить все JobDocs по jobId
        /// </summary>
        Task<List<JobDoc>> GetAllJobDocsByJobIdAsync(int jobId);

        /// <summary>
        /// Получить JobDocs по jobId с фильтрами
        /// </summary>
        Task<List<JobDoc>> GetJobDocsByJobIdWithFiltersAsync(
            int jobId,
            int? siteState = null,
            int? docState = null,
            int? partCountState = null,
            string? documentType = null,
            string? cleanedModel = null);

        /// <summary>
        /// Получить JobDocs по списку ID
        /// </summary>
        Task<List<JobDoc>> GetJobDocsByIdsAsync(int[] ids);

    }

}
