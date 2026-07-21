using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Recruitment.Infrastructure.AI
{
    public class AzureOpenAiAdapter : IAiAdapter
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<AzureOpenAiAdapter> _logger;

        public AzureOpenAiAdapter(
            HttpClient client,
            IConfiguration config,
            ILogger<AzureOpenAiAdapter> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var provider = (_config["AI:Provider"] ?? "openai").Trim();
            var isAzure = provider.Equals("azure", StringComparison.OrdinalIgnoreCase)
                || provider.Equals("azureopenai", StringComparison.OrdinalIgnoreCase);

            var endpoint = isAzure
                ? BuildAzureEndpoint()
                : _config["AI:OpenAI:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";

            var apiKey = isAzure
                ? _config["AI:Azure:ApiKey"]
                : _config["AI:OpenAI:ApiKey"] ?? _config["OpenAI:ApiKey"];

            var model = isAzure
                ? _config["AI:Azure:Deployment"] ?? _config["AI:Azure:Model"]
                : _config["AI:OpenAI:Model"] ?? _config["OpenAI:Model"];

            if (string.IsNullOrWhiteSpace(apiKey)
                || string.IsNullOrWhiteSpace(endpoint)
                || string.IsNullOrWhiteSpace(model))
            {
                _logger.LogWarning(
                    "AI provider {Provider} is selected but endpoint, API key, or model/deployment is missing.",
                    provider);
                return isAzure
                    ? "[ai-not-configured] Set AI:Azure:Endpoint, AI:Azure:ApiKey, and AI:Azure:Deployment."
                    : "[ai-not-configured] Set AI:OpenAI:ApiKey and AI:OpenAI:Model.";
            }

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            if (isAzure)
            {
                request.Headers.Add("api-key", apiKey);
            }
            else
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }

            request.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    model,
                    messages = new[] { new { role = "user", content = prompt } }
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("AI response status: {Status}. Body: {Body}", response.StatusCode, body);

            if (!response.IsSuccessStatusCode)
            {
                return $"[ai-error] Provider returned {(int)response.StatusCode} {response.StatusCode}.";
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
            catch (JsonException)
            {
                // Return raw provider body below.
            }

            return body;
        }

        private string BuildAzureEndpoint()
        {
            var configuredEndpoint = (_config["AI:Azure:Endpoint"] ?? _config["AI:Azure:Url"] ?? string.Empty)
                .Trim()
                .TrimEnd('/');

            if (string.IsNullOrWhiteSpace(configuredEndpoint))
            {
                return string.Empty;
            }

            if (configuredEndpoint.Contains("/chat/completions", StringComparison.OrdinalIgnoreCase))
            {
                return configuredEndpoint;
            }

            if (configuredEndpoint.EndsWith("/openai/v1", StringComparison.OrdinalIgnoreCase))
            {
                return $"{configuredEndpoint}/chat/completions";
            }

            var deployment = _config["AI:Azure:Deployment"] ?? _config["AI:Azure:Model"];
            var apiVersion = _config["AI:Azure:ApiVersion"] ?? "2024-10-21";

            if (string.IsNullOrWhiteSpace(deployment))
            {
                return $"{configuredEndpoint}/openai/v1/chat/completions";
            }

            return $"{configuredEndpoint}/openai/deployments/{Uri.EscapeDataString(deployment)}/chat/completions?api-version={Uri.EscapeDataString(apiVersion)}";
        }
    }
}
