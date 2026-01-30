using IMS.Domain.Products;
using IMS.Domain.Categories;
using IMS.Domain.Inventories;

namespace IMS.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IInventoryRepository Inventories { get; }
        public Task<int> Complete(CancellationToken cancellationToken);
    }
}
