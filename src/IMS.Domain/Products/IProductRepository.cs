using IMS.Domain.Abstractions;

namespace IMS.Domain.Products
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken);
        Task<bool> IsSkuUniqueAsync(string sku, CancellationToken cancellationToken);
    }
}
