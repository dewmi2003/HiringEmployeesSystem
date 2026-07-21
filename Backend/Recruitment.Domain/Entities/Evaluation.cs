using System;

namespace Recruitment.Domain.Entities
{
    public class Evaluation
    {
        public Guid Id { get; set; }

        // Foreign Key to Interview (existing)
        public Guid InterviewId { get; set; }
        public virtual Interview? Interview { get; set; }

        // Foreign Key to Recruiter (who evaluates)
        public Guid InterviewerId { get; set; }
        public virtual Recruiter? Interviewer { get; set; }

        // Foreign Key to Application
        public Guid ApplicationId { get; set; }
        public virtual Application? Application { get; set; }

        // Foreign Key to Candidate
        public Guid CandidateId { get; set; }
        public virtual Candidate? Candidate { get; set; }

        // Foreign Key to HiringManager (User)
        public Guid? HiringManagerId { get; set; }
        public virtual User? HiringManager { get; set; }

        // Phase 2 scoring criteria (0-100 each)
        public int TechnicalScore { get; set; }
        public int CommunicationScore { get; set; }
        public int ExperienceScore { get; set; }
        public int CultureFitScore { get; set; }

        // Computed
        public double OverallScore { get; set; }
        public string Recommendation { get; set; } = string.Empty;

        // Legacy single score (kept for backward compatibility)
        public int Score { get; set; }
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
