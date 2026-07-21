using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class HiringDecisionRepository 
        : Repository<HiringDecision>, IHiringDecisionRepository
    {

        


        public HiringDecisionRepository(
            ApplicationDbContext context)
            : base(context)
        {
           
        }



      public override async Task<List<HiringDecision>> GetAllAsync()
        {
            return await _context.HiringDecisions
                .Include(x => x.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(x => x.Application)
                    .ThenInclude(a => a.Job)
                .Include(x => x.DecidedByUser)
                .ToListAsync();
        }



        public override async Task<HiringDecision?> GetByIdAsync(Guid id)
        {
            return await _context.HiringDecisions
                .Include(x => x.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(x => x.Application)
                    .ThenInclude(a => a.Job)
                .Include(x => x.DecidedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }



       public async Task<List<HiringDecision>> GetByApplicationIdAsync(
    Guid applicationId)
        {
            return await _context.HiringDecisions
                .Where(x => x.ApplicationId == applicationId)
                .Include(x => x.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(x => x.Application)
                    .ThenInclude(a => a.Job)
                .Include(x => x.DecidedByUser)
                .ToListAsync();
        }



        public override async Task AddAsync(HiringDecision decision)
        {
            await _context.HiringDecisions.AddAsync(decision);
            await _context.SaveChangesAsync();
        }



        public override async Task UpdateAsync(HiringDecision decision)
        {
            _context.HiringDecisions.Update(decision);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(Guid id)
        {
            var decision =
                await _context.HiringDecisions
                .FirstOrDefaultAsync(x => x.Id == id);


            if (decision != null)
            {
                _context.HiringDecisions.Remove(decision);

                await _context.SaveChangesAsync();
            }
        }
    }
}
