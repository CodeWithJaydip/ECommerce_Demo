using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Role entity
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table name
        builder.ToTable("Roles");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(255);

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        var seedCreatedAt = new DateTime(2026, 01, 12, 00, 00, 00, DateTimeKind.Utc);

        builder.HasData(
            new Role 
            { 
                Id = 2, 
                Name = "Super Admin", 
                Description = "Full system access with all administrative privileges", 
                CreatedAt = seedCreatedAt, 
                IsActive = true 
            },
            new Role 
            { 
                Id = 3, 
                Name = "Seller", 
                Description = "Can create and manage products, view orders, manage inventory", 
                CreatedAt = seedCreatedAt, 
                IsActive = true 
            },
            new Role 
            { 
                Id = 4, 
                Name = "Buyer", 
                Description = "Can browse products, place orders, and manage personal account", 
                CreatedAt = seedCreatedAt, 
                IsActive = true 
            }
        );
    }
}
