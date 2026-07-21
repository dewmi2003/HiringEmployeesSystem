using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Infrastructure.Calendar
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly HttpClient _httpClient;
        private readonly CalendarSettings _settings;
        private readonly ILogger<GoogleCalendarService> _logger;

        public GoogleCalendarService(
            HttpClient httpClient,
            IOptions<CalendarSettings> settings,
            ILogger<GoogleCalendarService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> CreateEventAsync(
            string title,
            DateTime start,
            DateTime end,
            string email)
        {
            var token = await GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return "[calendar-not-configured] Set CalendarSettings:ClientId, CalendarSettings:ClientSecret, CalendarSettings:RefreshToken, and CalendarSettings:CalendarId.";
            }

            var calendarId = GetCalendarId();
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://www.googleapis.com/calendar/v3/calendars/{Uri.EscapeDataString(calendarId)}/events?sendUpdates=all");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    summary = title,
                    description = "Created by Recruitment Management System.",
                    start = new
                    {
                        dateTime = ToGoogleDateTime(start),
                        timeZone = _settings.TimeZone
                    },
                    end = new
                    {
                        dateTime = ToGoogleDateTime(end),
                        timeZone = _settings.TimeZone
                    },
                    attendees = string.IsNullOrWhiteSpace(email)
                        ? Array.Empty<object>()
                        : new object[] { new { email } }
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Google Calendar event creation failed. Status={StatusCode}, Body={Body}",
                    response.StatusCode,
                    responseBody);

                return $"[calendar-error] Google Calendar returned {(int)response.StatusCode} {response.StatusCode}.";
            }

            using var document = JsonDocument.Parse(responseBody);
            return document.RootElement.TryGetProperty("htmlLink", out var htmlLink)
                ? htmlLink.GetString() ?? "Google calendar event created"
                : "Google calendar event created";
        }

        public async Task<bool> CheckAvailabilityAsync(
            DateTime start,
            DateTime end)
        {
            var token = await GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return true;
            }

            var calendarId = GetCalendarId();
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://www.googleapis.com/calendar/v3/freeBusy");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    timeMin = ToGoogleDateTime(start),
                    timeMax = ToGoogleDateTime(end),
                    timeZone = _settings.TimeZone,
                    items = new[] { new { id = calendarId } }
                }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Google Calendar free/busy check failed. Status={StatusCode}, Body={Body}",
                    response.StatusCode,
                    responseBody);
                return true;
            }

            using var document = JsonDocument.Parse(responseBody);
            if (!document.RootElement.TryGetProperty("calendars", out var calendars)
                || !calendars.TryGetProperty(calendarId, out var calendar)
                || !calendar.TryGetProperty("busy", out var busy))
            {
                return true;
            }

            return busy.GetArrayLength() == 0;
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_settings.ClientId)
                || string.IsNullOrWhiteSpace(_settings.ClientSecret)
                || string.IsNullOrWhiteSpace(_settings.RefreshToken))
            {
                _logger.LogWarning("Google Calendar is selected but OAuth credentials are not configured.");
                return null;
            }

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["refresh_token"] = _settings.RefreshToken,
                ["grant_type"] = "refresh_token"
            });

            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Google OAuth token refresh failed. Status={StatusCode}, Body={Body}",
                    response.StatusCode,
                    body);
                return null;
            }

            using var document = JsonDocument.Parse(body);
            return document.RootElement.TryGetProperty("access_token", out var accessToken)
                ? accessToken.GetString()
                : null;
        }

        private string GetCalendarId()
        {
            return string.IsNullOrWhiteSpace(_settings.CalendarId)
                ? "primary"
                : _settings.CalendarId;
        }

        private static string ToGoogleDateTime(DateTime value)
        {
            var normalized = value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(value, DateTimeKind.Local)
                : value;

            return normalized.ToString("O");
        }
    }
}
