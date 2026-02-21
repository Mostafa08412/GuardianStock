using FluentEmail.Core;
using GuardianStock.Application.Common.Errors;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Infrastructure.EmailServices.EmailTemplates;
using GuardianStock.Infrastructure.EmailServices.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace GuardianStock.Infrastructure.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail fluentEmail;
        private readonly SmtpSettings smtpSettings;
        private readonly ILogger<EmailService> logger;
        private readonly IConfiguration _configuration;
        private readonly string _frontendBaseUrl;

        public EmailService(IConfiguration configuration, IFluentEmail fluentEmail, IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            this.fluentEmail = fluentEmail;
            this.smtpSettings = smtpSettings.Value;
            this.logger = logger;
            this._configuration = configuration;
            _frontendBaseUrl = _configuration.GetSection("FrontendSettings").GetValue<string>("BaseUrl") ?? "#";
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
                DashboardUrl = $"{_frontendBaseUrl}/inventories/{inventoryId}",
                AlertTime = DateTime.UtcNow
            };
            string templatePath = Path.Combine(AppContext.BaseDirectory, "EmailServices", "EmailTemplates", "LowStockEmail.cshtml");

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            var result = await fluentEmail
                 .To(smtpSettings.FromEmail)
                 .BCC(to.Select(X => new FluentEmail.Core.Models.Address(X)))
                 .Subject($"[Inventory Alert] Low Stock Warning: {productName} — Immediate Replenishment Required")
                 .UsingTemplateFromFile(templatePath, emailModel)
                 .SendAsync(cancellationToken);

            if (!result.Successful)
            {
                var errorMessage = string.Join(',', result.ErrorMessages);
                logger.LogWarning("Sending Low Stock Email Failed: Reason {reason}", errorMessage);
                return Result.Failure(ApplicationErrors.EmailErrors.SendFailed(errorMessage));
            }
            logger.LogInformation("Low stock alert email sent successfully for product {ProductName}", productName);

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
                LoginUrl = $"{_frontendBaseUrl}/login"
            };

            string templatePath = Path.Combine(AppContext.BaseDirectory, "EmailServices", "EmailTemplates", "UserCreatedEmail.cshtml");

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            var result = await fluentEmail
                .To(to)
                .Subject($"Welcome to the Team, {fullName} — Your Account Is Ready")
                .UsingTemplateFromFile(templatePath, emailModel)
                .SendAsync(cancellationToken);

            if (!result.Successful)
            {
                var errorMessage = string.Join(',', result.ErrorMessages);
                logger.LogWarning("Sending User Created Email Failed: Reason {reason}", errorMessage);
                return Result.Failure(ApplicationErrors.EmailErrors.SendFailed(errorMessage));
            }

            logger.LogInformation("User Created Email sent successfully to {Email}", email);

            return Result.Success();
        }


        public async Task SendForgetPasswordEmailAsync(string to, string name, string emailAddress, string otp, CancellationToken cancellationToken)
        {

            ForgetPasswordEmailModel model = new ForgetPasswordEmailModel
            {
                Name = name,
                EmailAddress = emailAddress,
                Otp = otp
            };

            string subject = "Password Reset Request — Secure Verification Code Enclosed";

            string templatePath = Path.Combine(AppContext.BaseDirectory, "EmailServices", "EmailTemplates", "ForgetPasswordEmailTemplate.cshtml");

            ServicePointManager.FindServicePoint(new Uri($"http://{smtpSettings.SmtpPort}")).ConnectionLimit = 1;

            await fluentEmail
                .To(to)
                .Subject(subject)
                .UsingTemplateFromFile(templatePath, model)
                .SendAsync(cancellationToken);



        }
    }
}
