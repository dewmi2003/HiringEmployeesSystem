using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class Recruiter
    {
        public Guid Id { get; set; }
        
        // Foreign Key to User
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Foreign Key to Company
        public Guid CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        // Navigation properties
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
        public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
