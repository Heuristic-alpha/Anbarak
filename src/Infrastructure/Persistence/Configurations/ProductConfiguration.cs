using Anbarak.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anbarak.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", Constance.Schemas.WAREHOUSE);

        builder.HasKey(product => product.Id);

        builder.HasIndex(product => product.Sku).HasDatabaseName("UX_Products_Sku").IsUnique();

        builder.Property(product => product.Sku).HasMaxLength(Product.SkuMaxLength).IsRequired();

        builder.Property(product => product.Name).HasMaxLength(Product.NameMaxLength).IsRequired();

        builder.Property(product => product.Description).HasMaxLength(Product.DescriptionMaxLength);

        builder.Property(product => product.UnitOfMeasure).HasConversion<string>().HasMaxLength(32).IsRequired();

        builder.Property(product => product.IsActive).IsRequired();
    }
}
