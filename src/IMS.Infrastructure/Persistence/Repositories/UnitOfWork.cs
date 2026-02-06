using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
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

        protected ApplicationDbContext dbContext;

        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;

        public UnitOfWork(ApplicationDbContext dbContext,
            ICurrentUser currentUser,
            IDateTime dateTime,
            IMediator mediator,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IInventoryRepository inventoryRepository,
            ITransactionRepository transactionRepository) // Inject the repository
        {

            this.dbContext = dbContext;
            _currentUser = currentUser;
            _dateTime = dateTime;
            _mediator = mediator;
            Products = productRepository;
            Categories = categoryRepository;
            Inventories = inventoryRepository;
            Transactions = transactionRepository;
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
            foreach (var entry in dbContext.ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(X => X.UpdatedOnUTC).CurrentValue = _dateTime.UTCNow;
                    entry.Property(X => X.UpdatedBy).CurrentValue = _currentUser.UserId;

                }

                if (entry.State == EntityState.Added)
                {
                    entry.Property(X => X.CreatedOnUTC).CurrentValue = _dateTime.UTCNow;
                    entry.Property(X => X.CreatedBy).CurrentValue = _currentUser.UserId;


                    entry.Property(X => X.UpdatedOnUTC).CurrentValue = _dateTime.UTCNow;
                    entry.Property(X => X.UpdatedBy).CurrentValue = _currentUser.UserId;


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
