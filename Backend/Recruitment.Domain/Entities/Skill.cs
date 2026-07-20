using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for Many-to-Many join table
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    }
}
