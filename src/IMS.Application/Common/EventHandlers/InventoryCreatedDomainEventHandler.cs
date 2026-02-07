using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Inventories.Domain_Events;
using IMS.Domain.StockHistories;
using MediatR;

namespace IMS.Application.Common.EventHandlers
{
    internal class InventoryCreatedDomainEventHandler : INotificationHandler<InventoryCreatedDomainEvent>
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IDateTime dateTime;

        public InventoryCreatedDomainEventHandler(IUnitOfWork unitOfWork, IDateTime dateTime)
        {
            this.unitOfWork = unitOfWork;
            this.dateTime = dateTime;
        }

        public Task Handle(InventoryCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var history = StockHistory.Create(notification.InventoryId, null, dateTime.UTCNow, notification.InitialStock);

            unitOfWork.StockHistories.Add(history);

            return Task.CompletedTask;
        }
    }
}
