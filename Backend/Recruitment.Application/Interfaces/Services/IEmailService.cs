using Recruitment.Application.DTOs.Email;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<EmailSendResult> SendEmailAsync(
            string to,
            string subject,
            string body);
    }
}
