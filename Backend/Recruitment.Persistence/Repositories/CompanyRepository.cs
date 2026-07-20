using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;
using System.Threading.Tasks;

namespace Recruitment.Persistence.Repositories
{
    public class CompanyRepository
        : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Company?> GetByNameAsync(string name)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}