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
    }
}
