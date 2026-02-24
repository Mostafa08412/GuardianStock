using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Inventories;
using GuardianStock.Domain.Products;
using GuardianStock.Domain.StockHistories;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Users;

namespace GuardianStock.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IInventoryRepository Inventories { get; }
        public ITransactionRepository Transactions { get; }
        public IStockHistoryRepository StockHistories { get; }
        public IUserRepository Users { get; }



        public Task<int> Complete(CancellationToken cancellationToken);

        public Task BeginTransactionAsync(CancellationToken cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken);

        public Task RollBackAsync(CancellationToken cancellationToken);
    }
}
