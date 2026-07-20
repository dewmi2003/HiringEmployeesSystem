using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Recruitment.Infrastructure.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string html)
        {
            _logger.LogInformation("MockEmailService.SendAsync to={To} subject={Subject}", to, subject);
            return Task.CompletedTask;
        }
    }
}
