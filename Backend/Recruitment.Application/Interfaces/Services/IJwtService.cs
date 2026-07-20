using Recruitment.Domain.Entities;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, string roleName);
    }
}
