using KameraData.Data.Dtos;

namespace WebApi_MaskService.Interface
{
    public interface IJobClient
    {
        Task<JobDto?> GetJobByIdAsync(int jobId);
    }
}
