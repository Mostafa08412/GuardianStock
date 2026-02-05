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

            if (inventory == null)
            {
                logger.LogCritical("Inventory record missing for ProductId: {ProductId}. Critical sync issue between transactions and inventory.",
                    notification.ProductId);
                return;
            }

            var previousStock = inventory.Quantity;

            if (notification.Type == TransactionType.Sale)
            {
                inventory.Ship(notification.Quantity);

                logger.LogInformation(
                    "Inventory Reduced: Product {ProductId} | Sold: {Quantity} | Stock: {PreviousStock} -> {CurrentStock}",
                    notification.ProductId, notification.Quantity, previousStock, inventory.Quantity);
            }
            else
            {
                inventory.Restock(notification.Quantity);

                logger.LogInformation(
                    "Inventory Restocked: Product {ProductId} | Added: {Quantity} | Stock: {PreviousStock} -> {CurrentStock}",
                    notification.ProductId, notification.Quantity, previousStock, inventory.Quantity);
            }


            if (inventory.Quantity <= 0)
            {
                logger.LogWarning("Product {ProductId} is now OUT OF STOCK.", notification.ProductId);
            }
        }
    }

}
