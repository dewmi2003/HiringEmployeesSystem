using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Recruitment.Infrastructure.AI
{
    public class GitHubModelsAdapter : IAiAdapter
    {
        private const string DefaultEndpoint = "https://models.github.ai/inference";
        private const string DefaultModel = "openai/gpt-4.1";

        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<GitHubModelsAdapter> _logger;

        public GitHubModelsAdapter(
            HttpClient client,
            IConfiguration config,
            ILogger<GitHubModelsAdapter> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var endpoint = BuildChatCompletionsEndpoint(
                _config["AI:GitHub:Endpoint"] ?? DefaultEndpoint);
            var token = _config["AI:GitHub:Token"];
            var model = _config["AI:GitHub:Model"] ?? DefaultModel;

            if (string.IsNullOrWhiteSpace(token)
                || string.IsNullOrWhiteSpace(endpoint)
                || string.IsNullOrWhiteSpace(model))
            {
                _logger.LogWarning("GitHub Models is selected but endpoint, token, or model is missing.");
                return "[ai-not-configured] Set AI:GitHub:Endpoint, AI:GitHub:Token, and AI:GitHub:Model.";
            }

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            request.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    model,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    temperature = 0.2,
                    stream = false
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("GitHub Models response status: {Status}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GitHub Models error response: {Body}", body);
                return $"[ai-error] GitHub Models returned {(int)response.StatusCode} {response.StatusCode}.";
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message)
                        && message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? string.Empty;
                    }

                    if (first.TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? string.Empty;
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "GitHub Models response was not valid JSON.");
            }

            return body;
        }

        private static string BuildChatCompletionsEndpoint(string endpoint)
        {
            var normalized = (endpoint ?? string.Empty).Trim().TrimEnd('/');
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return string.Empty;
            }

            return normalized.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase)
                ? normalized
                : $"{normalized}/chat/completions";
        }
    }
}
