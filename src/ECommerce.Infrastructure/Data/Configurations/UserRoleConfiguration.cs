using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for UserRole junction entity
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Table name
        builder.ToTable("UserRoles");

        // Composite primary key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Properties
        builder.Property(ur => ur.IsActive)
            .HasDefaultValue(true);

        builder.Property(ur => ur.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("IX_UserRoles_UserId");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRoles_RoleId");

        // Unique constraint to prevent duplicate user-role combinations
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRoles_User_Role_Unique");
    }
}
