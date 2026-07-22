using System.Net;
using System.Net.Mail;
using Recruitment.Application.DTOs.Email;
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



        public async Task<EmailSendResult> SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            var host = _settings.EffectiveHost;
            var provider = string.IsNullOrWhiteSpace(host)
                ? "SMTP"
                : $"SMTP:{host}:{_settings.Port}";

            if (string.IsNullOrWhiteSpace(to))
            {
                return new EmailSendResult(
                    false,
                    "Recipient email is required.",
                    provider);
            }

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
                return new EmailSendResult(
                    false,
                    "SMTP settings are incomplete. Check EmailSettings Host, Port, Username, Password, and FromEmail.",
                    provider);
            }

            try
            {
                using var client = new SmtpClient(
                    host,
                    _settings.Port);

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(
                    _settings.Username,
                    _settings.Password);



                var mail =
                    new MailMessage();


                mail.From =
                    new MailAddress(
                        _settings.FromEmail,
                        _settings.FromName);


                mail.To.Add(to);

                mail.Subject = subject;

                mail.Body = body;

                mail.IsBodyHtml = true;



                await client.SendMailAsync(mail);

                _logger.LogInformation(
                    "Email sent successfully. To={To}, Subject={Subject}",
                    to,
                    subject);

                return new EmailSendResult(
                    true,
                    "Email sent.",
                    provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Email send failed. To={To}, Subject={Subject}",
                    to,
                    subject);

                return new EmailSendResult(
                    false,
                    $"SMTP send failed: {BuildErrorMessage(ex)}",
                    provider);
            }
        }

        private static string BuildErrorMessage(Exception ex)
        {
            if (ex.InnerException == null
                || string.Equals(
                    ex.Message,
                    ex.InnerException.Message,
                    StringComparison.OrdinalIgnoreCase))
            {
                return ex.Message;
            }

            return $"{ex.Message} {ex.InnerException.Message}";
        }
    }
}
