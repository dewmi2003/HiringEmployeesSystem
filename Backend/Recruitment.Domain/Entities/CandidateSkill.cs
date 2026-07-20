using System;

namespace Recruitment.Domain.Entities
{
    public class CandidateSkill
    {
        // Many-to-Many Join Table Keys
        public Guid CandidateId { get; set; }
        public virtual Candidate? Candidate { get; set; }

        public Guid SkillId { get; set; }
        public virtual Skill? Skill { get; set; }
    }
}
