
using Recruitment.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IRecruiterRepository
        : IRepository<Recruiter>
    {
        Task<Recruiter?> GetByUserIdAsync(Guid userId);
    }
}