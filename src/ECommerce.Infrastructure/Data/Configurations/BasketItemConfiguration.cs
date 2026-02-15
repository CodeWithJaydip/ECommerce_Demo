using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.ToTable("BasketItems");

        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.Quantity)
            .IsRequired();

        builder.Property(bi => bi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(bi => bi.IsActive)
            .HasDefaultValue(true);

        builder.Property(bi => bi.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(bi => bi.Basket)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bi => bi.Product)
            .WithMany()
            .HasForeignKey(bi => bi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint: one entry per product per basket
        builder.HasIndex(bi => new { bi.BasketId, bi.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_BasketItems_Basket_Product");

        builder.HasIndex(bi => bi.BasketId)
            .HasDatabaseName("IX_BasketItems_BasketId");

        builder.HasIndex(bi => bi.ProductId)
            .HasDatabaseName("IX_BasketItems_ProductId");
    }
}
