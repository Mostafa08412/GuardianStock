using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Inventories;

namespace GuardianStock.Domain.Inventories
{
    public record LowStockAlertTriggeredDomainEvent : IDomainEvent
    {
        public LowStockAlertTriggeredDomainEvent(Guid productId, Guid inventoryId, DateTime triggeredAtUTC, int lowStockThreshold, int quantity, Inventory inventory)
        {
            ProductId = productId;
            InventoryId = inventoryId;
            TriggeredAtUTC = triggeredAtUTC;
            Threshold = lowStockThreshold;
            CurrentQuantity = quantity;
            Inventory = inventory;
        }
        public Inventory Inventory { get; init; }
        public Guid ProductId { get; init; }

        public Guid InventoryId { get; init; }

        public DateTime TriggeredAtUTC { get; init; }

        public int Threshold { get; init; }

        public int CurrentQuantity { get; init; }
    }
}
