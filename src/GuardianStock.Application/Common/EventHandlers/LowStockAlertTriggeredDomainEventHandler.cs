using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.LowStockAlerts.Commands.SendLowStockEmail;
using GuardianStock.Domain.Inventories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GuardianStock.Application.Common.EventHandlers
{
    internal class LowStockAlertTriggeredDomainEventHandler : INotificationHandler<LowStockAlertTriggeredDomainEvent>
    {
        private readonly IEmailService emailService;

        private readonly IIdentityService identityService;

        private readonly IApplicationDbContext applicationDbContext;

        private readonly ILogger<LowStockAlertTriggeredDomainEventHandler> logger;

        private readonly IBackgroundJobWorker backgroundJobWorker;

        public LowStockAlertTriggeredDomainEventHandler(IEmailService emailService, IIdentityService identityService, IApplicationDbContext applicationDbContext, ILogger<LowStockAlertTriggeredDomainEventHandler> logger, IBackgroundJobWorker backgroundJobWorker)
        {
            this.emailService = emailService;
            this.identityService = identityService;
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
            this.backgroundJobWorker = backgroundJobWorker;
        }

        public async Task Handle(LowStockAlertTriggeredDomainEvent notification, CancellationToken cancellationToken)
        {

            logger.LogInformation("Processing low stock alert for ProductId: {ProductId}. Current Quantity: {CurrentQuantity}",
            notification.ProductId, notification.CurrentQuantity);



            SendLowStockEmailCommand command = new SendLowStockEmailCommand
            {
                ProductId = notification.ProductId,
                InventoryId = notification.InventoryId
            };

            backgroundJobWorker.EnqueueSendLowStockEmailJob(command);


        }
    }
}
