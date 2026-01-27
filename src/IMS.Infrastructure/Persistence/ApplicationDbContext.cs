using IMS.Application.Common.Interfaces;
using IMS.Domain.Users;
using IMS.Infrastructure.Persistence.Identity;
using IMS.Infrastructure.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {


        public DbSet<User> BusinessUsers { get; private set; }
        public DbSet<RefreshToken> RefreshTokens { get; private set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

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
