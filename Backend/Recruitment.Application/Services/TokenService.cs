using System.Security.Cryptography;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;


namespace Recruitment.Application.Services
{
    public class TokenService : ITokenService
    {

        private readonly IRefreshTokenRepository _repository;



        public TokenService(
            IRefreshTokenRepository repository)
        {
            _repository = repository;
        }



        public async Task<string> GenerateRefreshTokenAsync(
            Guid userId)
        {

            var random =
                Convert.ToBase64String(
                    RandomNumberGenerator
                    .GetBytes(64));



            var token =
                new RefreshToken
                {

                    Id = Guid.NewGuid(),

                    UserId = userId,

                    Token = random,

                    CreatedAt = DateTime.UtcNow,

                    ExpiresAt =
                    DateTime.UtcNow.AddDays(7)

                };



            await _repository.AddAsync(token);



            return random;

        }





        public async Task<bool> ValidateRefreshTokenAsync(
            string token)
        {

            var data =
                await _repository.GetAsync(token);



            if(data == null)
                return false;



            if(data.IsRevoked)
                return false;



            if(data.ExpiresAt < DateTime.UtcNow)
                return false;



            return true;

        }





        public async Task RevokeRefreshTokenAsync(
            string token)
        {

            await _repository.RevokeAsync(token);

        }

    }
}