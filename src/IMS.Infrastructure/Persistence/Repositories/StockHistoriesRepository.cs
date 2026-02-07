using IMS.Domain.StockHistories;

namespace IMS.Infrastructure.Persistence.Repositories
{

    public class StockHistoriesRepository : BaseRepository<StockHistory>, IStockHistoryRepository
    {
        public StockHistoriesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
