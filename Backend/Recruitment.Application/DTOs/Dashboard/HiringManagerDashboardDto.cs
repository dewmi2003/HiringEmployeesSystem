namespace Recruitment.Application.DTOs.Dashboard
{
    public class HiringManagerDashboardDto
    {
        public int PendingEvaluations { get; set; }

        public int TotalInterviews { get; set; }

        public int PendingInterviews { get; set; }

        public int TotalHiringDecisions { get; set; }

        public int TotalOffers { get; set; }

        public int TotalHiredCandidates { get; set; }


        public List<CandidateRankingDto> TopCandidates { get; set; }
            = new();
        

        public List<InterviewSummaryDto> UpcomingInterviews { get; set; }
            = new();
    }
}