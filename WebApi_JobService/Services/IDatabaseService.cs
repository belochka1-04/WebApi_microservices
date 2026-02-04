using KameraData.Data.Dtos;
using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_JobService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<IEnumerable<JobDescriptionAndNote>> GetJobsAsync();
        Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID);
        Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status);
        Task UpdateDescriptionAndNotesDescriptionAsync(int jobId, string fulltext);
        Task UpdateDescriptionAndNotesPicCountAsync(int jobId, int pic_count);
        Task InsertDescriptionAndNotesDescriptionAsync(int jobId, string fulltext);
        Task<Job> GetJobByIdAsync(int jobId);
        Task<IEnumerable<Job>> GetJobByLink(string jobLink);
        Task<Job> AddJobByTgAsync(int Id);
        Task<JobDto?> GetJobDtoByIdAsync(int jobId);

        Task<int> GetTodaysOperationsCountAsync(int userId);

        Task<Job> CreateJobFromTextAsync(int userId, string? content, string? brand);

        Task<Job> CreateJobFromImageAsync(CreateJobFromImageDto dto);

        Task<Job?> GetFullJobByIdAsync(int jobId);

    }

}
