using IMS.Domain.Categories;
using IMS.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder
                .HasIndex(X => X.Sku)
                .IsUnique();

            builder.Property(X => X.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(X => X.Sku)
               .IsRequired()
               .HasMaxLength(25);

            builder.Property(X => X.Description)
               .IsRequired()
               .HasMaxLength(1500);

            builder.Property(X => X.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");


            builder
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(X => X.CategoryId);


        }
    }
}
