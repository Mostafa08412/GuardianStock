namespace GuardianStock.Infrastructure.EmailServices.Settings
{
    public class SmtpSettings
    {

        public const string SectionName = "SmtpSettings";

        public string SmtpHost { get; init; } = string.Empty;
        public int SmtpPort { get; init; }
        public bool UseSSL { get; init; }
        public bool UseCredentials { get; init; }
        public string FromEmail { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;

    }
}
