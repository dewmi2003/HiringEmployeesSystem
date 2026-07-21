using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class ApplicationStatusHistoryRepository : Repository<ApplicationStatusHistory>, IApplicationStatusHistoryRepository
    {
        public ApplicationStatusHistoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<ApplicationStatusHistory>> GetByApplicationIdAsync(Guid applicationId)
        {
            return await _context.Set<ApplicationStatusHistory>()
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
