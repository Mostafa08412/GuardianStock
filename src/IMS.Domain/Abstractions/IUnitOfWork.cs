using IMS.Domain.Products;
using IMS.Domain.Categories;

namespace IMS.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public Task<int> Complete(CancellationToken cancellationToken);
    }
}
