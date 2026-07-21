using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Authentication;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(
            IAuthService authService,
            ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        /// <summary>Login with email and password. Returns a JWT token.</summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>Register a new candidate or recruiter account.</summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return StatusCode(201, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
        RefreshTokenDto dto)
        {

        await _tokenService
        .RevokeRefreshTokenAsync(dto.Token);


        return Ok(new
        {
        message="Logged out successfully"
        });

        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
        RefreshTokenDto dto)
        {


        var valid =
        await _tokenService
        .ValidateRefreshTokenAsync(dto.Token);



        if(!valid)
        return Unauthorized();



        return Ok(new
        {
        message="Generate new JWT here"
        });

        }
    }
}
