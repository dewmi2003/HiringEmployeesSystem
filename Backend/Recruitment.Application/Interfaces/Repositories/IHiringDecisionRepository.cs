using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IHiringDecisionRepository
    {
        Task<List<HiringDecision>> GetAllAsync();

        Task<HiringDecision?> GetByIdAsync(Guid id);

        Task<List<HiringDecision>> GetByApplicationIdAsync(Guid applicationId);

        Task AddAsync(HiringDecision decision);

        Task UpdateAsync(HiringDecision decision);

        Task DeleteAsync(Guid id);
    }
}