using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IHiringDecisionRepository : IRepository<HiringDecision>
    {
        Task<IEnumerable<HiringDecision>> GetByApplicationIdAsync(Guid applicationId);
    }
}
