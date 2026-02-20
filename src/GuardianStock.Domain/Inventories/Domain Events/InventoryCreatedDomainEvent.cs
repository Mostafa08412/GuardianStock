using GuardianStock.Domain.Abstractions;

namespace GuardianStock.Domain.Inventories.Domain_Events
{
    public class InventoryCreatedDomainEvent : IDomainEvent
    {
        public InventoryCreatedDomainEvent(Guid inventoryId, int initialStock)
        {
            InventoryId = inventoryId;
            InitialStock = initialStock;
        }

        public Guid InventoryId { get; init; }
        public int InitialStock { get; init; }





    }
}
