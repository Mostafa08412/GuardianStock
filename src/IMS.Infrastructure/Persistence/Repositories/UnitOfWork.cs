using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.StockHistories;
using IMS.Domain.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IMS.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        public IProductRepository Products { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IInventoryRepository Inventories { get; private set; }
        public ITransactionRepository Transactions { get; private set; }

        protected IDbContextTransaction _currentTransaction { get; private set; }

        public IStockHistoryRepository StockHistories { get; private set; }

        protected ApplicationDbContext dbContext;

        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;

        public UnitOfWork(IProductRepository products, ICategoryRepository categories, IInventoryRepository inventories, ITransactionRepository transactions, IStockHistoryRepository stockHistories, ApplicationDbContext dbContext, ICurrentUser currentUser, IDateTime dateTime, IMediator mediator)
        {
            Products = products;
            Categories = categories;
            Inventories = inventories;
            Transactions = transactions;
            StockHistories = stockHistories;
            this.dbContext = dbContext;
            _currentUser = currentUser;
            _dateTime = dateTime;
            _mediator = mediator;
        }

        public async Task<int> Complete(CancellationToken cancellationToken)
        {
            AuditAddedAndModifiedEntries();

            var result = await dbContext.SaveChangesAsync(cancellationToken);

            while (true)
            {
                var aggregateEntitiesWithDomainEvents =
                   dbContext.ChangeTracker.Entries<Aggregate>()
                   .Select(X => X.Entity)
                   .Where(X => X.DomainEvents.Any()).ToList();

                if (!aggregateEntitiesWithDomainEvents.Any()) break;

                await PublishAggregatesDomainEvents(aggregateEntitiesWithDomainEvents, cancellationToken);
            }



            return await dbContext.SaveChangesAsync(cancellationToken);
        }


        private void AuditAddedAndModifiedEntries()
        {
            //Search for all added entities 
            var seedCreatedOrUpdatedBy = Guid.CreateVersion7().ToString();

            foreach (var entry in dbContext.ChangeTracker.Entries<IAuditable>())
            {

                entry.Property(X => X.UpdatedBy).CurrentValue =
                    entry.Entity.UpdatedBy == default ? seedCreatedOrUpdatedBy : entry.Entity.UpdatedBy;

                entry.Property(X => X.UpdatedOnUTC).CurrentValue =
                 entry.Entity.UpdatedOnUTC == default ? _dateTime.UTCNow : entry.Entity.UpdatedOnUTC;


                if (entry.State == EntityState.Added)
                {
                    entry.Property(X => X.CreatedBy).CurrentValue =
                        entry.Entity.CreatedBy == default ? seedCreatedOrUpdatedBy : entry.Entity.CreatedBy;

                    entry.Property(X => X.CreatedOnUTC).CurrentValue =
                       entry.Entity.CreatedOnUTC == default ? _dateTime.UTCNow : entry.Entity.CreatedOnUTC;


                }
            }
        }


        private async Task PublishAggregatesDomainEvents(List<Aggregate> aggregates, CancellationToken cancellationToken)
        {


            foreach (var entityWithDomainEvents in aggregates)
            {
                foreach (var domainEVent in entityWithDomainEvents.DomainEvents)
                {
                    await _mediator.Publish(domainEVent, cancellationToken);
                }
                entityWithDomainEvents.ClearDomainEvents();
            }

        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {

            _currentTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }

        public async Task RollBackAsync(CancellationToken cancellationToken)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
    }

}
