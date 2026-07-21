using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.Infrastructure.Email
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;


        public EmailService(
            IOptions<EmailSettings> settings,
            ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }



        public async Task SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            var host = _settings.EffectiveHost;
            if (string.IsNullOrWhiteSpace(host)
                || _settings.Port <= 0
                || string.IsNullOrWhiteSpace(_settings.Username)
                || string.IsNullOrWhiteSpace(_settings.Password)
                || string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                _logger.LogWarning(
                    "Email not sent because SMTP settings are not configured. To={To}, Subject={Subject}",
                    to,
                    subject);
                return;
            }

            using var client = new SmtpClient(
                host,
                _settings.Port);


            client.EnableSsl = true;


            client.Credentials =
                new NetworkCredential(
                    _settings.Username,
                    _settings.Password);



            var mail =
                new MailMessage();


            mail.From =
                new MailAddress(
                    _settings.FromEmail);


            mail.To.Add(to);

            mail.Subject = subject;

            mail.Body = body;

            mail.IsBodyHtml = true;



            await client.SendMailAsync(mail);
        }
    }
}
