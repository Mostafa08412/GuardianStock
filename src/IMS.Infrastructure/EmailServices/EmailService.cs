using FluentEmail.Core;
using IMS.Application.Common.Interfaces;
using IMS.Infrastructure.Email_Services.Options;
using IMS.Infrastructure.EmailServices.EmailTemplates;
using Microsoft.Extensions.Options;
using System.Net;

namespace IMS.Infrastructure.Email_Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail fluentEmail;
        private readonly SmtpSettings smtpSettings;

        public EmailService(IFluentEmail fluentEmail, IOptions<SmtpSettings> smtpSettings)
        {
            this.fluentEmail = fluentEmail;
            this.smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await fluentEmail.To(to).Body(body).Subject(subject).SendAsync();
        }

        public async Task SendLowStockEmailAsync(
         IEnumerable<string> to,
         string productName,
         string sku,
         int currentQuantity,
         int threshold,
         CancellationToken cancellationToken)
        {

            var emailModel = new LowStockEmailModel
            {
                ProductName = productName,
                Sku = sku,
                CurrentQuantity = currentQuantity,
                Threshold = threshold,
                DashboardUrl = "https://your-app.com/admin/inventory",
                AlertTime = DateTime.UtcNow
            };


            string templatePath = "IMS.Infrastructure.EmailServices.EmailTemplates.LowStockEmail.cshtml";

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            var result = await fluentEmail
                 .To(smtpSettings.FromEmail)
                 .BCC(to.Select(X => new FluentEmail.Core.Models.Address(X)))
                 .Subject($"URGENT: Low Stock - {productName}")
                 .UsingTemplateFromEmbedded(templatePath, emailModel, typeof(EmailService).Assembly)
                 .SendAsync(cancellationToken);
        }
    }
}
