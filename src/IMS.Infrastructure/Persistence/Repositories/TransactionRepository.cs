using IMS.Domain.Transactions;
using IMS.Infrastructure.Persistence.Repositories;

namespace IMS.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
