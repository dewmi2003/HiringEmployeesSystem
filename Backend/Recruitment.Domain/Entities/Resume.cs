using System;

namespace Recruitment.Domain.Entities
{
    public class Resume
    {
        public Guid Id { get; set; }

        // Foreign Key to Candidate
        public Guid CandidateId { get; set; }
        public virtual Candidate? Candidate { get; set; }

        public string FilePath { get; set; } = string.Empty;
        public string ParsedText { get; set; } = string.Empty;
        public int AiScore { get; set; }

        // Associated Date
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
