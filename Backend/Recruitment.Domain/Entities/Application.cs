using System;

namespace Recruitment.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }


        public Guid CandidateId { get; set; }

        public Guid JobId { get; set; }


        public Candidate Candidate { get; set; } = null!;

        public Job Job { get; set; } = null!;


        public DateTime AppliedDate { get; set; }

        public string Status { get; set; } = "Pending";

 public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }
}