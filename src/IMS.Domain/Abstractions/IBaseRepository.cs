using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Abstractions
{
    public interface IBaseRepository<T> where T : Aggregate
    {

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        public void AddRange(IEnumerable<T> entities);
        public void Add(T entity);
        public void Delete(T entity);
        public void Update(T entity);
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    }
}
