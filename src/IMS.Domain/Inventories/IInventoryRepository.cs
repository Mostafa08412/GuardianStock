using IMS.Domain.Abstractions;

namespace IMS.Domain.Inventories
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken ct);
        Task<bool> IsAvailableStockAsync(Guid productId, int quantity, CancellationToken ct);
    }
}
