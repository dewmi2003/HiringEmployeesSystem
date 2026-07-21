using Recruitment.Application.Interfaces.Services;


namespace Recruitment.Infrastructure.Calendar
{

public class GoogleCalendarService 
    : ICalendarService
{


public async Task<string> CreateEventAsync(
string title,
DateTime start,
DateTime end,
string email)
{


// Google Calendar API integration
// Add Google Calendar API credentials here


await Task.CompletedTask;


return "Google calendar event created";

}



public async Task<bool> CheckAvailabilityAsync(
DateTime start,
DateTime end)
{

await Task.CompletedTask;


return true;

}


}

}