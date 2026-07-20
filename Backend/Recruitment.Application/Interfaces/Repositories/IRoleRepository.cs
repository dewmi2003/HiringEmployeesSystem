using Recruitment.Domain.Entities;
using System.Threading.Tasks;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
    }
}