using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class Candidate
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual User? User { get; set; }


        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Bio { get; set; }

        public string? Experience { get; set; }

        public string? Education { get; set; }



        public virtual ICollection<Resume> Resumes { get; set; }
            = new List<Resume>();


        public virtual ICollection<Application> Applications { get; set; }
            = new List<Application>();


        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; }
            = new List<CandidateSkill>();
    }
}