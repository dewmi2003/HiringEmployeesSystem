namespace Recruitment.Application.DTOs.Dashboard
{
    public class InterviewSummaryDto
    {
        public Guid InterviewId { get; set; }


        public string CandidateName { get; set; }
            = string.Empty;


        public string JobTitle { get; set; }
            = string.Empty;


        public DateTime InterviewDate { get; set; }


        public string Status { get; set; }
            = string.Empty;
    }
}