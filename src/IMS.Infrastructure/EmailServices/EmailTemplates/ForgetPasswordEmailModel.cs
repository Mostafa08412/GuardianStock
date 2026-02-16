namespace IMS.Infrastructure.EmailServices.EmailTemplates
{
    public class ForgetPasswordEmailModel
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Otp { get; set; }

    }
}
