using GuardianStock.Domain.Transactions;
using GuardianStock.Infrastructure.Persistence;

namespace GuardianStock.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
