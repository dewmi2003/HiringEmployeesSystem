using System.Threading.Tasks;

namespace Recruitment.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string html);
    }
}
