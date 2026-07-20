using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class SkillRepository : Repository<Skill>, ISkillRepository
    {
        public SkillRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
