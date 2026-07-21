namespace Recruitment.Application.Interfaces.Services
{
    public interface ICalendarService
    {

        Task<string> CreateEventAsync(
            string title,
            DateTime start,
            DateTime end,
            string email);


        Task<bool> CheckAvailabilityAsync(
            DateTime start,
            DateTime end);

    }
}