using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class InterviewRepository : Repository<Interview>, IInterviewRepository
    {
        public InterviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Interview?> GetByIdAsync(Guid id)
        {
            return await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a!.Candidate)
                        .ThenInclude(c => c!.User)
                .Include(i => i.Application)
                    .ThenInclude(a => a!.Job)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public override async Task<List<Interview>> GetAllAsync()
        {
            return await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a!.Candidate)
                        .ThenInclude(c => c!.User)
                .Include(i => i.Application)
                    .ThenInclude(a => a!.Job)
                .OrderByDescending(i => i.ScheduledDate)
                .ToListAsync();
        }
    }
}
