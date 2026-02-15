using BasketEntity = ECommerce.Domain.Entities.Basket;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Basket.Interfaces;

public interface IBasketRepository
{
    Task<BasketEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<BasketEntity?> GetByUserIdForUpdateAsync(int userId, CancellationToken cancellationToken = default);
    Task<BasketEntity> CreateAsync(BasketEntity basket, CancellationToken cancellationToken = default);
    Task<BasketItem?> GetItemAsync(int basketId, int productId, CancellationToken cancellationToken = default);
    Task<BasketItem?> GetItemForUpdateAsync(int basketId, int productId, CancellationToken cancellationToken = default);
    Task<BasketItem?> GetInactiveItemForUpdateAsync(int basketId, int productId, CancellationToken cancellationToken = default);
    Task<BasketItem> AddItemAsync(BasketItem item, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(BasketItem item, CancellationToken cancellationToken = default);
    Task ClearItemsAsync(int basketId, CancellationToken cancellationToken = default);
    Task<int> GetItemCountAsync(int userId, CancellationToken cancellationToken = default);
}
