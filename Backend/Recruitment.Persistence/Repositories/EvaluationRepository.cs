using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence.Repositories
{
    public class EvaluationRepository : Repository<Evaluation>, IEvaluationRepository
    {
        public EvaluationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
