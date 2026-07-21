namespace Recruitment.Application.DTOs.Audit
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public string IpAddress { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
