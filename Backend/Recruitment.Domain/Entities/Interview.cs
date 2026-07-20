using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class Interview
    {
        public Guid Id { get; set; }

        // Foreign Key to Application
        public Guid ApplicationId { get; set; }
        public virtual Application? Application { get; set; }

        public DateTime ScheduledDate { get; set; }
        public string Location { get; set; } = string.Empty; // e.g. "Zoom", "Teams", "On-site"

        // Navigation properties
        public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
