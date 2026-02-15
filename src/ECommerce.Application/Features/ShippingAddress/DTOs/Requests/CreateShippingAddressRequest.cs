using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Constants;

namespace ECommerce.Application.Features.ShippingAddress.DTOs.Requests;

public record CreateShippingAddressRequest
{
    [Required(ErrorMessage = ShippingAddressConstants.FullNameRequired)]
    [StringLength(ShippingAddressConstants.FullNameMaxLength, ErrorMessage = ShippingAddressConstants.FullNameLengthInvalid)]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = ShippingAddressConstants.AddressLine1Required)]
    [StringLength(ShippingAddressConstants.AddressLineMaxLength, ErrorMessage = ShippingAddressConstants.AddressLineLengthInvalid)]
    public string AddressLine1 { get; init; } = string.Empty;

    [StringLength(ShippingAddressConstants.AddressLineMaxLength, ErrorMessage = ShippingAddressConstants.AddressLineLengthInvalid)]
    public string? AddressLine2 { get; init; }

    [Required(ErrorMessage = ShippingAddressConstants.CityRequired)]
    [StringLength(ShippingAddressConstants.CityMaxLength, ErrorMessage = ShippingAddressConstants.CityLengthInvalid)]
    public string City { get; init; } = string.Empty;

    [Required(ErrorMessage = ShippingAddressConstants.StateRequired)]
    [StringLength(ShippingAddressConstants.StateMaxLength, ErrorMessage = ShippingAddressConstants.StateLengthInvalid)]
    public string State { get; init; } = string.Empty;

    [Required(ErrorMessage = ShippingAddressConstants.PostalCodeRequired)]
    [StringLength(ShippingAddressConstants.PostalCodeMaxLength, ErrorMessage = ShippingAddressConstants.PostalCodeLengthInvalid)]
    public string PostalCode { get; init; } = string.Empty;

    [Required(ErrorMessage = ShippingAddressConstants.CountryRequired)]
    [StringLength(ShippingAddressConstants.CountryMaxLength, ErrorMessage = ShippingAddressConstants.CountryLengthInvalid)]
    public string Country { get; init; } = string.Empty;

    [StringLength(ShippingAddressConstants.PhoneNumberMaxLength, ErrorMessage = ShippingAddressConstants.PhoneNumberLengthInvalid)]
    public string? PhoneNumber { get; init; }

    public bool IsDefault { get; init; }
}
