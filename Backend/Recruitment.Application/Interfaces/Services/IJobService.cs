using Recruitment.Application.DTOs.Jobs;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<List<JobListDto>> GetAllActiveJobsAsync(string? location = null, string? title = null);
        Task<JobDetailDto?> GetJobByIdAsync(Guid jobId);
        Task<Guid> CreateJobAsync(Guid recruiterId, CreateJobDto dto);
        Task UpdateJobAsync(Guid jobId, UpdateJobDto dto);
        Task DeleteJobAsync(Guid jobId);
    }
}
