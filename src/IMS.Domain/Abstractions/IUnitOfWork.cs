using IMS.Domain.Categories;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.Transactions;

namespace IMS.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IInventoryRepository Inventories { get; }
        public ITransactionRepository Transactions { get; }

        public Task<int> Complete(CancellationToken cancellationToken);

        public Task BeginTransactionAsync(CancellationToken cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken);

        public Task RollBackAsync(CancellationToken cancellationToken);
    }
}
