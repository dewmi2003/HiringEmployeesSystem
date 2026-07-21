using System;

namespace Recruitment.Domain.Entities
{
    /// <summary>Phase 3 - Audit trail of every application status change.</summary>
    public class ApplicationStatusHistory
    {
        public Guid Id { get; set; }

        // Foreign Key to Application
        public Guid ApplicationId { get; set; }
        public virtual Application? Application { get; set; }

        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;

        // The user who made the change
        public Guid ChangedByUserId { get; set; }
        public virtual User? ChangedByUser { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string? Comments { get; set; }
    }
}
