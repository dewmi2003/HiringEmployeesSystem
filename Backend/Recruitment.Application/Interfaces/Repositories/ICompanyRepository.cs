using Recruitment.Domain.Entities;
using System.Threading.Tasks;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetByNameAsync(string name);
    }
}