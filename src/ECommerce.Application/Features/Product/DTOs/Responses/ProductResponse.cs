namespace ECommerce.Application.Features.Product.DTOs.Responses;

/// <summary>
/// DTO for product response
/// </summary>
public record ProductResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int SellerId { get; init; }
    public string SellerName { get; init; } = string.Empty;
    public string? ImagePath { get; init; }
    public string? SKU { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}
