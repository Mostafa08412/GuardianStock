using IMS.Domain.Categories;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.Transactions;
using IMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<User> BusinessUsers { get; }
        public DbSet<Product> Products { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Transaction> Transactions { get; }
        public DbSet<Inventory> Inventories { get; }

    }
}
