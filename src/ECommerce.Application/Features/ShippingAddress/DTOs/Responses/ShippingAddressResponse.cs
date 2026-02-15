namespace ECommerce.Application.Features.ShippingAddress.DTOs.Responses;

public record ShippingAddressResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public bool IsDefault { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
