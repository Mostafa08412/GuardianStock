using IMS.Domain.Products;

namespace IMS.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; }
        public Task<int> Complete(CancellationToken cancellationToken);
    }
}
