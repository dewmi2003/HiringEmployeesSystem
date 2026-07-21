
namespace Recruitment.Application.DTOs.Reports
{
    public class RecruitmentReportDto
    {
        public int TotalCandidates { get; set; }

        public int TotalJobs { get; set; }

        public int TotalApplications { get; set; }

        public int TotalInterviews { get; set; }

        public int TotalHired { get; set; }

        public double HiringRate { get; set; }
    }
}