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
         string inventoryId,
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
                DashboardUrl = $"http://localhost:8080/inventories/{inventoryId}",
                AlertTime = DateTime.UtcNow
            };
            string templatePath = Path.Combine(AppContext.BaseDirectory, "EmailServices", "EmailTemplates", "LowStockEmail.cshtml");

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            var result = await fluentEmail
                 .To(smtpSettings.FromEmail)
                 .BCC(to.Select(X => new FluentEmail.Core.Models.Address(X)))
                 .Subject($"URGENT: Low Stock - {productName}")
                 .UsingTemplateFromFile(templatePath, emailModel)
                 .SendAsync(cancellationToken);

            if (!result.Successful)
            {
                var errorMessage = string.Join(',', result.ErrorMessages);
                logger.LogWarning("Sending Low Stock Email Failed: Reason {reason}", errorMessage);
                return Result.Failure(new Error("Email.SendFailed", errorMessage, ErrorType.Failure));
            }
            logger.LogWarning("Sending Low Stock Email Succeeded");

            return Result.Success();
        }

        public async Task<Result> SendUserCreatedEmailAsync(
            string to,
            string fullName,
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            var emailModel = new UserCreatedEmailModel
            {
                FullName = fullName,
                Email = email,
                Password = password,
                LoginUrl = "https://your-app.com/login"
            };

            string templatePath = Path.Combine(AppContext.BaseDirectory, "EmailServices", "EmailTemplates", "UserCreatedEmail.cshtml");

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            var result = await fluentEmail
                .To(to)
                .Subject("Your Account Has Been Created")
                .UsingTemplateFromFile(templatePath, emailModel)
                .SendAsync(cancellationToken);

            if (!result.Successful)
            {
                var errorMessage = string.Join(',', result.ErrorMessages);
                logger.LogWarning("Sending User Created Email Failed: Reason {reason}", errorMessage);
                return Result.Failure(new Error("Email.SendFailed", errorMessage, ErrorType.Failure));
            }

            logger.LogInformation("User Created Email sent successfully to {Email}", email);

            return Result.Success();
        }
    }
}
