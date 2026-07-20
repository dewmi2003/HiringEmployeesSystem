using System;

namespace Recruitment.Domain.Entities
{
    public class Evaluation
    {
        public Guid Id { get; set; }

        // Foreign Key to Interview
        public Guid InterviewId { get; set; }
        public virtual Interview? Interview { get; set; }

        // Foreign Key to Recruiter (who evaluates)
        public Guid InterviewerId { get; set; }
        public virtual Recruiter? Interviewer { get; set; }

        public int Score { get; set; } // e.g. 1 to 5 or 1 to 10
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
