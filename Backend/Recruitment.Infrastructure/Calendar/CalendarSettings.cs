namespace Recruitment.Infrastructure.Calendar
{
    public class CalendarSettings
    {

        public string Provider { get; set; }
            = "Google";


        public string ClientId { get; set; }
            = string.Empty;


        public string ClientSecret { get; set; }
            = string.Empty;


        public string RefreshToken { get; set; }
            = string.Empty;


        public string CalendarId { get; set; }
            = "primary";


        public string TimeZone { get; set; }
            = "Asia/Colombo";

    }
}
