namespace ECommerce.Application.Features.Basket.DTOs.Responses;

public record BasketItemResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImagePath { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal CurrentPrice { get; init; }
    public int Quantity { get; init; }
    public decimal SubTotal { get; init; }
    public int Stock { get; init; }
    public bool IsAvailable { get; init; }
}
