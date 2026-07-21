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
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string ParsedText { get; set; } = string.Empty;
        public int AiScore { get; set; }
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime UploadedDate {get;set;}
        // Associated Date
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
