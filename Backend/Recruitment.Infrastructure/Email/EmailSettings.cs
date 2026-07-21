namespace Recruitment.Infrastructure.Email
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;

        public string Server { get; set; } = string.Empty;

        public int Port { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FromEmail { get; set; } = string.Empty;

        public string EffectiveHost =>
            string.IsNullOrWhiteSpace(Host)
                ? Server
                : Host;
    }
}
