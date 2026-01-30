using IMS.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
        {
            return !await _entity.AnyAsync(c => c.Name == name, ct);
        }
    }
}
