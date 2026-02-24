using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Inventories;
using GuardianStock.Domain.Products;
using GuardianStock.Domain.StockHistories;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Users;
using GuardianStock.Infrastructure.Persistence.Identity;
using GuardianStock.Infrastructure.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace GuardianStock.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>,Guid>, IApplicationDbContext
    {
        public DbSet<User> BusinessUsers { get; private set; }
        public DbSet<RefreshToken> RefreshTokens { get; private set; }
        public DbSet<Product> Products { get; private set; }
        public DbSet<Category> Categories { get; private set; }
        public DbSet<Inventory> Inventories { get; private set; }
        public DbSet<Transaction> Transactions { get; private set; }
        public DbSet<StockHistory> StockHistories { get; private set; }
        public DatabaseFacade DB { get; private set; }


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            this.DB = this.Database;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
