using IMS.Domain.Abstractions;

namespace IMS.Domain.Categories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<bool> IsNameUniqueAsync(string name, CancellationToken ct);

        Task<IEnumerable<(Guid, string)>> GetAllCategoriesNamesWithIdsAsync(CancellationToken ct);
    }
}
