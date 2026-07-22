using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestEmail(SendTestEmailDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.To))
            {
                return BadRequest("Recipient email is required.");
            }

            await _emailService.SendEmailAsync(
                dto.To,
                "Recruitment system email test",
                "<p>Email sending is configured for the recruitment system.</p>");

            return Ok(new { message = "Email test request completed." });
        }
    }

    public record SendTestEmailDto(string To);
}
