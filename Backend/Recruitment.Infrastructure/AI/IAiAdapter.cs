using System.Threading.Tasks;

namespace Recruitment.Infrastructure.AI
{
    public interface IAiAdapter
    {
        Task<string> GetCompletionAsync(string prompt);
    }
}
