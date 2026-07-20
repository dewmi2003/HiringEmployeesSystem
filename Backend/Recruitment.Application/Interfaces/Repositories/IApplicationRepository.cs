using ApplicationEntity = Recruitment.Domain.Entities.Application;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IApplicationRepository
    {
        Task<IEnumerable<ApplicationEntity>> GetAllAsync();

        Task<ApplicationEntity?> GetByIdAsync(Guid id);

        Task<IEnumerable<ApplicationEntity>> GetByJobIdAsync(Guid jobId);

        Task<IEnumerable<ApplicationEntity>> GetByCandidateIdAsync(Guid candidateId);

        Task AddAsync(ApplicationEntity application);

        Task UpdateAsync(ApplicationEntity application);

        Task DeleteAsync(Guid id);
    }
}