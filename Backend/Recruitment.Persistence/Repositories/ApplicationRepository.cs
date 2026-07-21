using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Persistence.Context;
using ApplicationEntity = Recruitment.Domain.Entities.Application;

namespace Recruitment.Persistence.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
       private readonly ApplicationDbContext _context;

public ApplicationRepository(ApplicationDbContext context)
{
    _context = context;
}

        public async Task<IEnumerable<ApplicationEntity>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .ToListAsync();
        }

        public async Task<ApplicationEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ApplicationEntity>> GetByJobIdAsync(Guid jobId)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Where(a => a.JobId == jobId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationEntity>> GetByCandidateIdAsync(Guid candidateId)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Where(a => a.CandidateId == candidateId)
                .ToListAsync();
        }

        public async Task AddAsync(ApplicationEntity application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationEntity application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
            }
        }
    }
}
