namespace GuardianStock.Infrastructure.EmailServices.Options
{
    public class SmtpSettings
    {

        public static string SectionName => "SmtpSettings";

        public string FromEmail { get; init; }

        public int SmtpPort { get; init; }

        public string SmtpHost { get; init; }

        public string Password { get; init; }

        public bool UseSSL { get; init; }

    }
}
