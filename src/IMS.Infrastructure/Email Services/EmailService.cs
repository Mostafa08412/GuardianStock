using FluentEmail.Core;
using IMS.Application.Common.Interfaces;

namespace IMS.Infrastructure.Email_Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            this.fluentEmail = fluentEmail;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await fluentEmail.To(to).Body(body).Subject(subject).SendAsync();
        }
    }
}
