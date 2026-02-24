namespace GuardianStock.Infrastructure.EmailServices.EmailTemplates
{
    public class ForgetPasswordEmailModel
    {
        public string Name { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;

    }
}
