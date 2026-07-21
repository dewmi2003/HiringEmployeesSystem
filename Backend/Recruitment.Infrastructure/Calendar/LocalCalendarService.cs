using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Infrastructure.Calendar
{
    public class LocalCalendarService : ICalendarService
    {
        public Task<string> CreateEventAsync(
            string title,
            DateTime start,
            DateTime end,
            string email)
        {
            return Task.FromResult("Local calendar event placeholder created");
        }

        public Task<bool> CheckAvailabilityAsync(
            DateTime start,
            DateTime end)
        {
            return Task.FromResult(true);
        }
    }
}
