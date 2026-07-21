using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class HiringDecisionRepository : Repository<HiringDecision>, IHiringDecisionRepository
    {
        public HiringDecisionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<HiringDecision>> GetByApplicationIdAsync(Guid applicationId)
        {
            return await _context.Set<HiringDecision>()
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.DecidedAt)
                .ToListAsync();
        }
    }
}
