using ECommerce.Application.Features.Basket.DTOs.Requests;
using ECommerce.Application.Features.Basket.DTOs.Responses;

namespace ECommerce.Application.Features.Basket.Interfaces;

public interface IBasketService
{
    Task<BasketResponse> GetBasketAsync(int userId, CancellationToken cancellationToken = default);
    Task<BasketResponse> AddItemAsync(int userId, AddBasketItemRequest request, CancellationToken cancellationToken = default);
    Task<BasketResponse> UpdateItemQuantityAsync(int userId, int productId, UpdateBasketItemRequest request, CancellationToken cancellationToken = default);
    Task<BasketResponse> RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default);
    Task<bool> ClearBasketAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> GetItemCountAsync(int userId, CancellationToken cancellationToken = default);
    Task<CheckoutSummaryResponse> GetCheckoutSummaryAsync(int userId, int? addressId, CancellationToken cancellationToken = default);
}
