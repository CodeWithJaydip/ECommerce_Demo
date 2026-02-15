namespace ECommerce.Application.Features.Basket.DTOs.Responses;

public record BasketResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public List<BasketItemResponse> Items { get; init; } = [];
    public decimal TotalAmount { get; init; }
    public int TotalItems { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
