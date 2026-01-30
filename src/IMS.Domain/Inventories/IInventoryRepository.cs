using IMS.Domain.Abstractions;

namespace IMS.Domain.Inventories
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken ct);
    }
}
