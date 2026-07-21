using System;

namespace Recruitment.Domain.Entities
{
    /// <summary>Phase 3 - Hiring Decision record.</summary>
    public class HiringDecision
    {
        public Guid Id { get; set; }

        // Foreign Key to Application
        public Guid ApplicationId { get; set; }
        public virtual Application? Application { get; set; }

        // The recruiter / hiring-manager who made the decision
        public Guid DecidedByUserId { get; set; }
        public virtual User? DecidedByUser { get; set; }

        /// <summary>Shortlisted | Rejected | OfferExtended | Hired</summary>
        public string Decision { get; set; } = string.Empty;

        public string? Comments { get; set; }

        public DateTime DecidedAt { get; set; } = DateTime.UtcNow;
    }
}
