using GuardianStock.Domain.Abstractions;

namespace GuardianStock.Domain.Inventories
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken ct);
        Task<bool> IsAvailableStockAsync(Guid productId, int quantity, CancellationToken ct);
    }
}
