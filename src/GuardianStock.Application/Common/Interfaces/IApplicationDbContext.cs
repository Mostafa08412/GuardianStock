using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Inventories;
using GuardianStock.Domain.Products;
using GuardianStock.Domain.StockHistories;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GuardianStock.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<User> BusinessUsers { get; }
        public DbSet<Product> Products { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Transaction> Transactions { get; }
        public DbSet<Inventory> Inventories { get; }
        public DbSet<StockHistory> StockHistories { get; }

        public DatabaseFacade DB { get; }
    }
}
