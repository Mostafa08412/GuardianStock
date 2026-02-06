using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.LowStockAlerts.Commands.SendLowStockEmail
{
    public class SendLowStockEmailCommandHandler : IRequestHandler<SendLowStockEmailCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IEmailService _emailService;

        private readonly IIdentityService _identityService;

        private readonly ILogger<SendLowStockEmailCommandHandler> _logger;

        public SendLowStockEmailCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService, IIdentityService identityService, ILogger<SendLowStockEmailCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task Handle(SendLowStockEmailCommand request, CancellationToken ct)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, ct);
            if (product is null)
            {
                _logger.LogWarning("Job Aborted: Product {ProductId} not found. Nothing to process.", request.ProductId);
                return;
            }

            var inventory = await _unitOfWork.Inventories.GetByProductIdAsync(request.ProductId, ct);
            if (inventory is null)
            {
                _logger.LogCritical("Job Aborted: Inventory record missing for Product Sku: {ProductSku}.", product.Sku);
                return;
            }

            if (inventory.LowStockAlert is null || inventory.LowStockAlert.IsNotificationSent)
            {
                _logger.LogInformation("Job Aborted: Low stock alert for {ProductSku} is null or already sent.", product.Sku);
                return;
            }

            var receivers = (await _identityService.GetUsersEmailsByRoleAsync(Roles.Admin)).Union(await _identityService.GetUsersEmailsByRoleAsync(Roles.Manager));
            if (receivers == null || !receivers.Any())
            {
                _logger.LogWarning("Job Suspended: No recipients found for Low Stock Alert: {ProductSku}.", product.Sku);
                return;
            }

            try
            {
                _logger.LogInformation("Attempting to send Low Stock Email to {Count} users for Sku: {Sku}", receivers.Count(), product.Sku);

                await _emailService.SendLowStockEmailAsync(
                    receivers,
                    product.Name,
                    product.Sku,
                    inventory.Quantity,
                    inventory.LowStockThreshold,
                    ct);

                inventory.ConfirmNotificationSent();


                _logger.LogInformation("Job Completed: Low Stock Email sent successfully for {ProductSku}.", product.Sku);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job Failed: Error occurred while sending email for {ProductSku}.", product.Sku);
                throw;
            }
        }
    }

}
