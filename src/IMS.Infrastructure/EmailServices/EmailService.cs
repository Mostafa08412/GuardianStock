using FluentEmail.Core;
using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;
using IMS.Infrastructure.EmailServices.EmailTemplates;
using IMS.Infrastructure.EmailServices.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace IMS.Infrastructure.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail fluentEmail;
        private readonly SmtpSettings smtpSettings;
        private readonly ILogger<EmailService> logger;

        public EmailService(IFluentEmail fluentEmail, IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            this.fluentEmail = fluentEmail;
            this.smtpSettings = smtpSettings.Value;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await fluentEmail.To(to).Body(body).Subject(subject).SendAsync();
        }

        public async Task<Result> SendLowStockEmailAsync(
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

            if (!result.Successful)
            {
                var errorMessage = string.Join(',', result.ErrorMessages);
                logger.LogWarning("Sending Low Stock Email Failed: Reason {reason}", errorMessage);
                return Result.Failure(new Error("Email.SendFialed", errorMessage, ErrorType.Failure));
            }
            logger.LogWarning("Sending Low Stock Email Succeeded");

            return Result.Success();
        }
    }
}
