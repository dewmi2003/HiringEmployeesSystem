using Recruitment.Application.DTOs.Authentication;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly IJwtService _jwtService;


        public AuthService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            ICandidateRepository candidateRepo,
            IJwtService jwtService)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _candidateRepo = candidateRepo;
            _jwtService = jwtService;
        }



        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email)
                       ?? throw new UnauthorizedAccessException(
                           "Invalid email or password.");


            if (!BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash))
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }


            var roleName = user.Role?.Name ?? "Candidate";


            var token = _jwtService.GenerateToken(
                user,
                roleName);


            return new AuthResponseDto(
                token,
                user.Email,
                user.FullName,
                roleName,
                DateTime.UtcNow.AddHours(8)
            );
        }





        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request)
        {
            var existingUser =
                await _userRepo.GetByEmailAsync(request.Email);


            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "Email is already registered.");
            }



            var role =
                await _roleRepo.GetByNameAsync(request.Role);



            if (role == null)
            {
                throw new InvalidOperationException(
                    $"Role '{request.Role}' does not exist.");
            }



            var user = new User
            {
                Id = Guid.NewGuid(),

                FullName = request.FullName,

                Email = request.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        request.Password),

                CreatedDate = DateTime.UtcNow,

                RoleId = role.Id
            };



            await _userRepo.AddAsync(user);



            // Create Candidate Profile automatically
            if (role.Name.Equals(
                    "Candidate",
                    StringComparison.OrdinalIgnoreCase))
            {

                var names = request.FullName.Split(' ');


                var candidate = new Candidate
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    FirstName = names[0],

                    LastName =
                        names.Length > 1
                        ? names[1]
                        : "",

                    Phone = "",

                    Address = null,

                    Bio = null,

                    Experience = null,

                    Education = null
                };


                await _candidateRepo.AddAsync(candidate);
            }




            var token =
                _jwtService.GenerateToken(
                    user,
                    role.Name);



            return new AuthResponseDto(
                token,
                user.Email,
                user.FullName,
                role.Name,
                DateTime.UtcNow.AddHours(8)
            );
        }
    }
}