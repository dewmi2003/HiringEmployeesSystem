using System;

namespace Recruitment.Domain.Entities
{
    public class HiringDecision
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid DecidedByUserId { get; set; }

        public string Decision { get; set; } = string.Empty;

        public string? Comments { get; set; }

        public DateTime DecidedAt { get; set; }


        public Application Application { get; set; } = null!;

        public User DecidedByUser { get; set; } = null!;
    }
}