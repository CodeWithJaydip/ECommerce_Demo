using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Category entity
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name
        builder.ToTable("Categories");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(255);

        builder.Property(c => c.ImagePath)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Name");

        // Seed data
        var seedCreatedAt = new DateTime(2026, 01, 12, 00, 00, 00, DateTimeKind.Utc);

        builder.HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Devices, gadgets, and accessories", CreatedAt = seedCreatedAt, IsActive = true },
            new Category { Id = 2, Name = "Fashion", Description = "Clothing, footwear, and accessories", CreatedAt = seedCreatedAt, IsActive = true },
            new Category { Id = 3, Name = "Home & Kitchen", Description = "Home essentials and kitchenware", CreatedAt = seedCreatedAt, IsActive = true },
            new Category { Id = 4, Name = "Beauty & Personal Care", Description = "Skincare, haircare, and personal care", CreatedAt = seedCreatedAt, IsActive = true },
            new Category { Id = 5, Name = "Sports & Outdoors", Description = "Sports gear and outdoor equipment", CreatedAt = seedCreatedAt, IsActive = true }
        );
    }
}

