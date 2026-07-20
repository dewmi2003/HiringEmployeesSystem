using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;
using System;
using System.Threading.Tasks;

namespace Recruitment.Persistence.Repositories
{
    public class RecruiterRepository
        : Repository<Recruiter>, IRecruiterRepository
    {
        private readonly ApplicationDbContext _context;

        public RecruiterRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<Recruiter?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Recruiters
                .Include(r => r.User)
                .Include(r => r.Company)
                .FirstOrDefaultAsync(
                    r => r.UserId == userId
                );
        }
    }
}