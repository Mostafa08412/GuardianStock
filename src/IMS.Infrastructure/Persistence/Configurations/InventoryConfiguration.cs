using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.StockHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Persistence.Configurations
{
    internal class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories");

            builder
                .HasIndex(X => X.ProductId)
                .IsUnique();

            builder.OwnsOne(X => X.LowStockAlert);

            builder
                .HasOne<Product>().WithOne()
                .HasForeignKey<Inventory>(X => X.ProductId);


            builder
                .HasMany<StockHistory>().WithOne()
                .HasForeignKey(X => X.InventoryId);

        }
    }
}
