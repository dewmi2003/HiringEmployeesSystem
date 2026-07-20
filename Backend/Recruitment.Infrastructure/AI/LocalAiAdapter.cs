using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Recruitment.Infrastructure.AI
{
    public class LocalAiAdapter : IAiAdapter
    {
        private readonly ILogger<LocalAiAdapter> _logger;

        public LocalAiAdapter(ILogger<LocalAiAdapter> logger)
        {
            _logger = logger;
        }

        public Task<string> GetCompletionAsync(string prompt)
        {
            _logger.LogInformation("LocalAiAdapter received prompt: {Prompt}", prompt);
            return Task.FromResult("[mock-ai-response] Summary for: " + prompt);
        }
    }
}
