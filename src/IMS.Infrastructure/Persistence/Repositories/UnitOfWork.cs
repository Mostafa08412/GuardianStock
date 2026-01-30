using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Products;
using IMS.Domain.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        public IProductRepository Products { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        protected ApplicationDbContext dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IDateTime _dateTime;
        private readonly IMediator _mediator;

        public UnitOfWork(ApplicationDbContext dbContext,
            ICurrentUser currentUser,
            IDateTime dateTime,
            IMediator mediator,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository) // Inject the repository
        {

            this.dbContext = dbContext;
            _currentUser = currentUser;
            _dateTime = dateTime;
            _mediator = mediator;
            Products = productRepository;
            Categories = categoryRepository;
        }

        public async Task<int> Complete(CancellationToken cancellationToken)
        {

            AuditAddedAndModfiedEntries();

            var aggregateEntitiesWithDomainEvents =
                dbContext.ChangeTracker.Entries<Aggregate>()
                .Select(X => X.Entity)
                .Where(X => X.DomainEvents.Any()).ToList();

            var result = await dbContext.SaveChangesAsync(cancellationToken);


            PublishAggregatesDomainEvents(aggregateEntitiesWithDomainEvents, cancellationToken);


            return await dbContext.SaveChangesAsync(cancellationToken);
        }


        private void AuditAddedAndModfiedEntries()
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

        private void PublishAggregatesDomainEvents(List<Aggregate> aggregates, CancellationToken cancellationToken)
        {

            foreach (var entityWithDomainEvents in aggregates)
            {
                foreach (var domainEVent in entityWithDomainEvents.DomainEvents)
                {
                    _mediator.Publish(domainEVent, cancellationToken);
                }
                entityWithDomainEvents.ClearDomainEvents();
            }

        }
    }

}
