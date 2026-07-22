using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Application.DTOs.Email;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Application.Services;

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

            var result = await _emailService.SendEmailAsync(
                dto.To,
                "Recruitment system email test",
                EmailTemplateBuilder.Test());

            return ToActionResult(result);
        }

        [HttpPost("test/welcome")]
        public async Task<IActionResult> SendWelcomeTestEmail(EmailTemplateTestDto dto)
        {
            var result = await _emailService.SendEmailAsync(
                dto.To,
                "Welcome to TalentAI",
                EmailTemplateBuilder.Welcome(dto.CandidateName));

            return ToActionResult(result);
        }

        [HttpPost("test/application")]
        public async Task<IActionResult> SendApplicationTestEmail(EmailTemplateTestDto dto)
        {
            var jobTitle = dto.JobTitle ?? "Software Developer";

            var result = await _emailService.SendEmailAsync(
                dto.To,
                $"Application received - {jobTitle}",
                EmailTemplateBuilder.ApplicationSubmitted(
                    dto.CandidateName,
                    jobTitle,
                    dto.CompanyName ?? "TalentAI",
                    DateTime.UtcNow));

            return ToActionResult(result);
        }

        [HttpPost("test/interview")]
        public async Task<IActionResult> SendInterviewTestEmail(EmailTemplateTestDto dto)
        {
            var start = DateTime.UtcNow.AddDays(2);
            var end = start.AddHours(1);
            var jobTitle = dto.JobTitle ?? "Software Developer";

            var result = await _emailService.SendEmailAsync(
                dto.To,
                $"Interview scheduled - {jobTitle}",
                EmailTemplateBuilder.InterviewScheduled(
                    dto.CandidateName,
                    jobTitle,
                    start,
                    end,
                    dto.Location ?? "Colombo",
                    null));

            return ToActionResult(result);
        }

        [HttpPost("test/offer")]
        public async Task<IActionResult> SendOfferTestEmail(EmailTemplateTestDto dto)
        {
            var jobTitle = dto.JobTitle ?? "Software Developer";

            var result = await _emailService.SendEmailAsync(
                dto.To,
                $"Job offer - {jobTitle}",
                EmailTemplateBuilder.OfferExtended(
                    dto.CandidateName,
                    jobTitle,
                    dto.CompanyName ?? "TalentAI",
                    dto.Comments));

            return ToActionResult(result);
        }

        private IActionResult ToActionResult(EmailSendResult result)
        {
            var response = new
            {
                sent = result.Sent,
                message = result.Message,
                provider = result.Provider
            };

            return result.Sent
                ? Ok(response)
                : StatusCode(StatusCodes.Status502BadGateway, response);
        }
    }

    public record SendTestEmailDto(string To);

    public record EmailTemplateTestDto(
        string To,
        string? CandidateName,
        string? JobTitle,
        string? CompanyName,
        string? Location,
        string? Comments
    );
}
