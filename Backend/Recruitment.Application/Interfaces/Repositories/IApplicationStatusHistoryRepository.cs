using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IApplicationStatusHistoryRepository : IRepository<ApplicationStatusHistory>
    {
        Task<IEnumerable<ApplicationStatusHistory>> GetByApplicationIdAsync(Guid applicationId);
    }
}
