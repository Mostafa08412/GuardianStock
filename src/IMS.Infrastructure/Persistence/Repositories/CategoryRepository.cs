using IMS.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<(Guid, string)>> GetAllCategoriesNamesWithIdsAsync(CancellationToken ct)
        {

            var query = _entity.AsNoTracking().AsQueryable().Select(X => new { X.Id, X.Name });

            var result = (await query.ToListAsync(ct)).Select(X => (X.Id, X.Name));

            return result;
        }

        public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
        {
            return !await _entity.AnyAsync(c => c.Name == name, ct);
        }
    }
}
