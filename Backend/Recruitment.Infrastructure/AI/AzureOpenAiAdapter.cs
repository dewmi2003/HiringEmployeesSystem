using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Recruitment.Infrastructure.AI
{
    public class AzureOpenAiAdapter : IAiAdapter
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<AzureOpenAiAdapter> _logger;

        public AzureOpenAiAdapter(HttpClient client, IConfiguration config, ILogger<AzureOpenAiAdapter> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;

            var apiKey = _config["AI:Azure:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            // Basic implementation against OpenAI compatible API (chat completions)
            var endpoint = _config["AI:Azure:Endpoint"] ?? _config["AI:Azure:Url"] ?? "https://api.openai.com/v1/chat/completions";
            var model = _config["AI:Azure:Model"] ?? "gpt-3.5-turbo";

            var payload = new
            {
                model = model,
                messages = new[] { new { role = "user", content = prompt } }
            };

            var json = JsonSerializer.Serialize(payload);
            var resp = await _client.PostAsync(endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
            var body = await resp.Content.ReadAsStringAsync();
            _logger.LogInformation("AI response status: {Status}. Body: {Body}", resp.StatusCode, body);

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? string.Empty;
                    }
                    if (first.TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? string.Empty;
                    }
                }
            }
            catch (JsonException)
            {
                // fall through
            }

            return body;
        }
    }
}
