using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key to Role
        public Guid RoleId { get; set; }
        public virtual Role? Role { get; set; }

        // One-to-One Relationships (Nullable since they depend on the User's role)
        public virtual Candidate? Candidate { get; set; }
        public virtual Recruiter? Recruiter { get; set; }

        // Notifications sent to the User
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}