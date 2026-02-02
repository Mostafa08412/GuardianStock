using IMS.Domain.Abstractions;

namespace IMS.Domain.Inventories
{
    public record LowStockAlertTriggeredDomainEvent : IDomainEvent
    {
        public LowStockAlertTriggeredDomainEvent(Guid productId, Guid inventoryId, DateTime triggeredAtUTC, int lowStockThreshold, int quantity)
        {
            ProductId = productId;
            InventoryId = inventoryId;
            TriggeredAtUTC = triggeredAtUTC;
            Threshold = lowStockThreshold;
            CurrentQuantity = quantity;
        }

        public Guid ProductId { get; init; }

        public Guid InventoryId { get; init; }

        public DateTime TriggeredAtUTC { get; init; }

        public int Threshold { get; init; }

        public int CurrentQuantity { get; init; }
    }
}
