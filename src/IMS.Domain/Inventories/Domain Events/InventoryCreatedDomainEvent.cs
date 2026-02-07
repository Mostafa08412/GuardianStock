using IMS.Domain.Abstractions;

namespace IMS.Domain.Inventories.Domain_Events
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
