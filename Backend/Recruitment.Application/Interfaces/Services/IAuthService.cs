using Recruitment.Application.DTOs.Authentication;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    }
}
