using System;

namespace Recruitment.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        // Foreign Key to User
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
