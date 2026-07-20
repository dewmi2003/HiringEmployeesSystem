using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class CandidateRepository
        : Repository<Candidate>, ICandidateRepository
    {

        public CandidateRepository(ApplicationDbContext context)
            : base(context)
        {
        }



        public async Task<Candidate?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }



        public override async Task<Candidate?> GetByIdAsync(Guid id)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.Id == id);
        }



        public override async Task<List<Candidate>> GetAllAsync()
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .ToListAsync();
        }
    }
}