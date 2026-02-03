using IMS.Domain.Abstractions;
using IMS.Domain.Transactions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Common.EventHandlers
{
    internal class TransactionCreatedDomainEventHandler : INotificationHandler<TransactionCreatedDomainEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<TransactionCreatedDomainEventHandler> logger;

        public TransactionCreatedDomainEventHandler(IUnitOfWork unitOfWork, ILogger<TransactionCreatedDomainEventHandler> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var inventory = await unitOfWork.Inventories.GetByProductIdAsync(notification.ProductId, cancellationToken);

            if (notification.Type == TransactionType.Sale)
            {
                inventory!.Ship(notification.Quantity);
                logger.LogInformation("Inventory stock of product with id {productId} reduced by {quantity} current stock is {currentStock} ", notification.ProductId, notification.Quantity, inventory.Quantity);
            }
            else
            {
                inventory!.Restock(notification.Quantity);
                logger.LogInformation("Inventory stock of product with id {productId} increased by {quantity} current stock is {currentStock} ", notification.ProductId, notification.Quantity, inventory.Quantity);


            }
        }
    }

}
