using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
