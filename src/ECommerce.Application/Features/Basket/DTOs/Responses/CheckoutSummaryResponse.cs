using ECommerce.Application.Features.ShippingAddress.DTOs.Responses;

namespace ECommerce.Application.Features.Basket.DTOs.Responses;

public record CheckoutSummaryResponse
{
    public BasketResponse Basket { get; init; } = null!;
    public ShippingAddressResponse? ShippingAddress { get; init; }
    public decimal SubTotal { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal TotalAmount { get; init; }
    public bool HasStockIssues { get; init; }
}
