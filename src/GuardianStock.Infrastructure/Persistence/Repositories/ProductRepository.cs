using GuardianStock.Domain.Products;
using GuardianStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct)
        {
            return await _entity.FirstOrDefaultAsync(x => x.Sku == sku, ct);
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, CancellationToken cancellationToken)
        {
            return !await _entity.AnyAsync(x => x.Sku == sku, cancellationToken);
        }


    }
}
