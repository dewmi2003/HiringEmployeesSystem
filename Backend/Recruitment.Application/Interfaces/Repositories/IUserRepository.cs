using System.Threading.Tasks;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
