using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        public string? Location { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key to Company
        public Guid CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        // Foreign Key to Recruiter
        public Guid RecruiterId { get; set; }
        public virtual Recruiter? Recruiter { get; set; }

        // Navigation properties
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}