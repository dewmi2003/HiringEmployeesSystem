using System;

namespace Recruitment.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        
        // Report data stored as serialized JSON or text
        public string Data { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
