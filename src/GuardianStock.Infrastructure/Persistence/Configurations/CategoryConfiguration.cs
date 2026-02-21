using GuardianStock.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuardianStock.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder
                .HasIndex(X => X.Name)
                .IsUnique();

            builder.Property(X => X.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(X => X.Description)
               .IsRequired()
               .HasMaxLength(500);

        }
    }
}
