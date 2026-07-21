using Recruitment.Domain.Entities;


namespace Recruitment.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {

        Task AddAsync(
            RefreshToken token);



        Task<RefreshToken?> GetAsync(
            string token);



        Task RevokeAsync(
            string token);

    }
}