using GuardianStock.Domain.Products;
using GuardianStock.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuardianStock.Infrastructure.Persistence.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder
                .HasIndex(X => new { X.ProductId, X.CreatedOnUTC })
                .IsUnique();

            builder.
                 Property(X => X.UnitPrice).HasPrecision(18, 2);

            builder
                 .HasOne<Product>().WithMany()
                 .HasForeignKey(X => X.ProductId);


        }
    }
}
