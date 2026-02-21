using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives;
using GuardianStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Infrastructure.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Aggregate
    {

        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _entity;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _entity.FindAsync(new object[] { id }, cancellationToken);
        }

        public void Add(T entity)
        {
            _entity.Add(entity);
        }

        public void Delete(T entity)
        {
            _entity.Remove(entity);
        }

        public void Update(T entity)
        {
            _entity.Update(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _entity.AddRange(entities);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _entity.AnyAsync(X => X.Id == id, cancellationToken);
        }


    }
}
