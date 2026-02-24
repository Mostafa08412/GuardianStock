using GuardianStock.Domain.StockHistories;
using GuardianStock.Infrastructure.Persistence;

namespace GuardianStock.Infrastructure.Persistence.Repositories
{

    public class StockHistoriesRepository : BaseRepository<StockHistory>, IStockHistoryRepository
    {
        public StockHistoriesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
