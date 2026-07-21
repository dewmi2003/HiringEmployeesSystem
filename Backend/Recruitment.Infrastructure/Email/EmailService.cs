using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.Infrastructure.Email
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _settings;


        public EmailService(
            IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }



        public async Task SendEmailAsync(
            string to,
            string subject,
            string body)
        {

            using var client = new SmtpClient(
                _settings.Host,
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