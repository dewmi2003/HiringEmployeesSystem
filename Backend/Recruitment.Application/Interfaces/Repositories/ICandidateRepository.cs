using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate?> GetByIdAsync(Guid id);

        Task<List<Candidate>> GetAllAsync();

        Task<Candidate?> GetByUserIdAsync(Guid userId);

        Task AddAsync(Candidate candidate);

        Task UpdateAsync(Candidate candidate);

        Task DeleteAsync(Candidate candidate);
    }
}