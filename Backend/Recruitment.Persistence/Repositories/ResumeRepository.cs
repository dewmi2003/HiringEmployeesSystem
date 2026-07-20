using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class ResumeRepository : Repository<Resume>, IResumeRepository
    {
        public ResumeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
