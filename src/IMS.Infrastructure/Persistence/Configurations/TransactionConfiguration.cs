using IMS.Domain.Products;
using IMS.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Persistence.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder
                .HasIndex(X => new { X.ProductId, X.CreatedOnUTC })
                .IsUnique();

            builder
                 .HasOne<Product>().WithMany()
                 .HasForeignKey(X => X.ProductId);


        }
    }
}
