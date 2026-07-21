using Recruitment.Application.Interfaces.Services;


namespace Recruitment.Infrastructure.Calendar
{

public class OutlookCalendarService 
    : ICalendarService
{


public async Task<string> CreateEventAsync(
string title,
DateTime start,
DateTime end,
string email)
{


await Task.CompletedTask;


return "Outlook calendar event created";

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