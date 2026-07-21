using ApplicationEntity = Recruitment.Domain.Entities.Application;
using Recruitment.Application.DTOs.Applications;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationEntity>> GetAllApplicationsAsync();

        Task<ApplicationEntity?> GetApplicationByIdAsync(Guid id);

        Task<IEnumerable<ApplicationDetailDto>> GetApplicationsByJobAsync(Guid jobId);

        Task<IEnumerable<ApplicationListDto>> GetApplicationsByCandidateAsync(Guid candidateId);

        Task<Guid> ApplyToJobAsync(Guid candidateId, CreateApplicationDto dto);

        Task UpdateStatusAsync(Guid applicationId, UpdateApplicationStatusDto dto);

        Task UpdateApplicationAsync(ApplicationEntity application);

        Task DeleteApplicationAsync(Guid id);
    }
}
