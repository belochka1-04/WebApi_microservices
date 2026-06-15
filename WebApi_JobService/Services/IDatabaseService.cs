using JobService.Domain.Entities;
using WebApi_JobService.Application.Dtos;

namespace WebApi_JobService.Services
{
    public interface IDatabaseService : KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        // job_description_and_notes
        Task<IEnumerable<JobDescriptionAndNote>> GetJobsAsync();
        Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID);
        Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status);
        Task UpdateDescriptionAndNotesDescriptionAsync(int jobId, string fulltext);
        Task UpdateDescriptionAndNotesPicCountAsync(int jobId, int pic_count);
        Task InsertDescriptionAndNotesDescriptionAsync(int jobId, string fulltext);

        Task MarkJobDescriptionsGoogleConfirmedAsync(string requestWord);

        // jobs
        Task<Job> GetJobByIdAsync(int jobId);
        Task<IEnumerable<Job>> GetJobByLink(string jobLink);
        Task<Job> AddJobByTgAsync(int id);

        // Пока можно оставить старый DTO, если он нужен внешнему REST
        Task<JobDto?> GetJobDtoByIdAsync(int jobId);

        Task<int> GetTodaysOperationsCountAsync(int userId);

        Task<Job> CreateJobFromTextAsync(int userId, string? content, string? brand);
        Task<Job> CreateJobFromImageAsync(CreateJobFromImageDto dto);

        Task<Job?> GetFullJobByIdAsync(int jobId);
    }
}