namespace Recruitment.Application.DTOs.Dashboard
{
    public class CandidateRankingDto
    {
        public Guid CandidateId { get; set; }

        public string CandidateName { get; set; }
            = string.Empty;


        public string JobTitle { get; set; }
            = string.Empty;


        public decimal Score { get; set; }


        public string Recommendation { get; set; }
            = string.Empty;
    }
}