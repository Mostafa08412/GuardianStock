namespace GuardianStock.Infrastructure.EmailServices.EmailTemplates
{
    public class UserCreatedEmailModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
    }
}
