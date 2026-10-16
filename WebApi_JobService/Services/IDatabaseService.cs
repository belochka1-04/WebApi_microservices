using KameraData.Data.Dtos;
using KameraData.Data.Models;
using KameraData.Data.Other;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_JobService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<IEnumerable<JobDescriptionAndNote>> GetJobsAsync();
        Task<IEnumerable<JobDescriptionAndNote>> ClaimJobsAsync(string workerId, int batchSize, int leaseSeconds);
        Task<bool> UpdateModelCheckerLeaseAsync(int id, ModelCheckerLeaseRequest request, string operation);
        Task<bool> RetryModelCheckerJobAsync(int id, string reason);
        Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID);
        Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status);
        Task UpdateDescriptionAndNotesDescriptionAsync(int jobId, string fulltext);
        Task UpdateDescriptionAndNotesPicCountAsync(int jobId, int pic_count);
        Task InsertDescriptionAndNotesDescriptionAsync(int jobId, string fulltext);

        Task MarkJobDescriptionsGoogleConfirmedAsync(string requestWord);
        Task<Job> GetJobByIdAsync(int jobId);
        Task<IEnumerable<Job>> GetJobByLink(string jobLink);
        Task<Job> AddJobByTgAsync(int Id);
        Task<JobDto?> GetJobDtoByIdAsync(int jobId);

        Task<int> GetTodaysOperationsCountAsync(int userId);

        Task<Job> CreateJobFromTextAsync(int userId, string? content, string? brand);

        Task<Job> CreateJobFromImageAsync(CreateJobFromImageDto dto);

        Task<Job?> GetFullJobByIdAsync(int jobId);
        Task InsertJobProcessingEventAsync(JobProcessingEventRequest request);
        Task<IEnumerable<JobProcessingEventRequest>> GetJobProcessingEventsAsync(int jobId);
        Task InsertBusinessEventAsync(BusinessEventRequest request);
        Task<IEnumerable<BusinessEventRequest>> GetBusinessEventsForJobAsync(int jobId);
        Task<IEnumerable<BusinessEventRequest>> GetBusinessEventsForUserAsync(int userId, int take);

    }

}
