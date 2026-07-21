namespace Recruitment.Application.DTOs.Analytics
{
    public class PerformanceAnalyticsDto
    {
        public int TotalApplications { get; set; }

        public int TotalHires { get; set; }

        public double SuccessRate { get; set; }
    }
}