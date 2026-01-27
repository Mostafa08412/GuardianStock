namespace IMS.Infrastructure.Email_Services.Options
{
    internal class SmtpSettings
    {

        public static string SectionName => "SmtpSettings";

        public string FromEmail { get; init; }

        public int SmtpPort { get; init; }

        public string SmtpHost { get; init; }

        public string Password { get; init; }

    }
}
