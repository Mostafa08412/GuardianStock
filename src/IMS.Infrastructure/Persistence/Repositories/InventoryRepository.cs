using IMS.Domain.Inventories;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken ct)
        {
            return await _entity.FirstOrDefaultAsync(i => i.ProductId == productId, ct);
        }

        public async Task<bool> IsAvailableStockAsync(Guid productId, int quantity, CancellationToken ct)
        {
            return (await _entity.FirstOrDefaultAsync(x => x.ProductId == productId, ct))!.Quantity >= quantity;
        }
    }
}
