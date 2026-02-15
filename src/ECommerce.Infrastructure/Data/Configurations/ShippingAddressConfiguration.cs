using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Constants;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.ToTable("ShippingAddresses");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.FullName)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.FullNameMaxLength);

        builder.Property(sa => sa.AddressLine1)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.AddressLineMaxLength);

        builder.Property(sa => sa.AddressLine2)
            .HasMaxLength(ShippingAddressConstants.AddressLineMaxLength);

        builder.Property(sa => sa.City)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.CityMaxLength);

        builder.Property(sa => sa.State)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.StateMaxLength);

        builder.Property(sa => sa.PostalCode)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.PostalCodeMaxLength);

        builder.Property(sa => sa.Country)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.CountryMaxLength);

        builder.Property(sa => sa.PhoneNumber)
            .HasMaxLength(ShippingAddressConstants.PhoneNumberMaxLength);

        builder.Property(sa => sa.IsDefault)
            .HasDefaultValue(false);

        builder.Property(sa => sa.IsActive)
            .HasDefaultValue(true);

        builder.Property(sa => sa.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(sa => sa.User)
            .WithMany()
            .HasForeignKey(sa => sa.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(sa => sa.UserId)
            .HasDatabaseName("IX_ShippingAddresses_UserId");
    }
}
