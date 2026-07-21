namespace Recruitment.Application.Interfaces.Services
{
    public interface ITokenService
    {

        Task<string> GenerateRefreshTokenAsync(
            Guid userId);



        Task<bool> ValidateRefreshTokenAsync(
            string token);



        Task RevokeRefreshTokenAsync(
            string token);

    }
}