using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// Requires SendGrid nuget package for production use
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Recruitment.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IConfiguration config, ILogger<SendGridEmailService> logger)
        {
            _apiKey = config["Azure:SendGrid:ApiKey"] ?? config["SendGrid__ApiKey"] ?? string.Empty;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("SendGrid API key not configured. Email not sent.");
                return;
            }

            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("no-reply@recruitment.local", "Recruitment"),
                Subject = subject,
                HtmlContent = html
            };
            msg.AddTo(new EmailAddress(to));

            var resp = await client.SendEmailAsync(msg);
            _logger.LogInformation("SendGrid response: {StatusCode}", resp.StatusCode);
        }
    }
}
