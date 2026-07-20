using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user, string roleName)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt__Key"]
                    ?? throw new InvalidOperationException("JWT key is not configured.")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role,               roleName),
                new Claim("fullName",                    user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var expiry = DateTime.UtcNow.AddHours(8);

            var token = new JwtSecurityToken(
                issuer:             _config["Jwt__Issuer"],
                audience:           _config["Jwt__Audience"],
                claims:             claims,
                expires:            expiry,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
