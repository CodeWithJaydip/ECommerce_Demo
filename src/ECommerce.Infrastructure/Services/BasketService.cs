using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Application.Features.Basket.DTOs.Requests;
using ECommerce.Application.Features.Basket.DTOs.Responses;
using ECommerce.Application.Features.Basket.Interfaces;
using ECommerce.Application.Features.Product.Interfaces;
using ECommerce.Application.Features.ShippingAddress.DTOs.Responses;
using ECommerce.Application.Features.ShippingAddress.Interfaces;
using ECommerce.Domain.Constants;
using ECommerce.Domain.Entities;
using BasketEntity = ECommerce.Domain.Entities.Basket;

namespace ECommerce.Infrastructure.Services;

public class BasketService(
    IBasketRepository basketRepository,
    IProductRepository productRepository,
    IShippingAddressRepository shippingAddressRepository,
    IUnitOfWork unitOfWork) : IBasketService
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IShippingAddressRepository _shippingAddressRepository = shippingAddressRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<BasketResponse> GetBasketAsync(int userId, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        if (basket == null)
        {
            return new BasketResponse
            {
                UserId = userId,
                Items = [],
                TotalAmount = 0,
                TotalItems = 0
            };
        }

        return MapToResponse(basket);
    }

    public async Task<BasketResponse> AddItemAsync(int userId, AddBasketItemRequest request, CancellationToken cancellationToken = default)
    {
        // Validate product
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException(BasketConstants.ProductNotFound);

        if (!product.IsActive)
        {
            throw new InvalidOperationException(BasketConstants.ProductNotAvailable);
        }

        if (product.Stock < request.Quantity)
        {
            throw new InvalidOperationException(BasketConstants.InsufficientStock);
        }

        // Get or create basket
        var basket = await _basketRepository.GetByUserIdForUpdateAsync(userId, cancellationToken);
        if (basket == null)
        {
            basket = new BasketEntity { UserId = userId };
            basket = await _basketRepository.CreateAsync(basket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Check if item already exists
        var existingItem = await _basketRepository.GetItemForUpdateAsync(basket.Id, request.ProductId, cancellationToken);
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + request.Quantity;
            if (newQuantity > BasketConstants.MaxQuantityPerItem)
            {
                newQuantity = BasketConstants.MaxQuantityPerItem;
            }

            if (product.Stock < newQuantity)
            {
                throw new InvalidOperationException(BasketConstants.InsufficientStock);
            }

            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Check for a soft-deleted item and reactivate it
            var inactiveItem = await _basketRepository.GetInactiveItemForUpdateAsync(basket.Id, request.ProductId, cancellationToken);
            if (inactiveItem != null)
            {
                inactiveItem.IsActive = true;
                inactiveItem.Quantity = request.Quantity;
                inactiveItem.UnitPrice = product.Price;
                inactiveItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var basketItem = new BasketItem
                {
                    BasketId = basket.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price
                };
                await _basketRepository.AddItemAsync(basketItem, cancellationToken);
            }
        }

        basket.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return refreshed basket
        var refreshedBasket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        return MapToResponse(refreshedBasket!);
    }

    public async Task<BasketResponse> UpdateItemQuantityAsync(int userId, int productId, UpdateBasketItemRequest request, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetByUserIdForUpdateAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException(BasketConstants.BasketNotFound);

        var item = await _basketRepository.GetItemForUpdateAsync(basket.Id, productId, cancellationToken)
            ?? throw new KeyNotFoundException(BasketConstants.BasketItemNotFound);

        // Validate stock
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException(BasketConstants.ProductNotFound);

        if (product.Stock < request.Quantity)
        {
            throw new InvalidOperationException(BasketConstants.InsufficientStock);
        }

        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        basket.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshedBasket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        return MapToResponse(refreshedBasket!);
    }

    public async Task<BasketResponse> RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetByUserIdForUpdateAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException(BasketConstants.BasketNotFound);

        var item = await _basketRepository.GetItemForUpdateAsync(basket.Id, productId, cancellationToken)
            ?? throw new KeyNotFoundException(BasketConstants.BasketItemNotFound);

        await _basketRepository.RemoveItemAsync(item, cancellationToken);
        basket.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshedBasket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        return MapToResponse(refreshedBasket!);
    }

    public async Task<bool> ClearBasketAsync(int userId, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetByUserIdForUpdateAsync(userId, cancellationToken);
        if (basket == null)
        {
            return true;
        }

        await _basketRepository.ClearItemsAsync(basket.Id, cancellationToken);
        basket.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> GetItemCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _basketRepository.GetItemCountAsync(userId, cancellationToken);
    }

    public async Task<CheckoutSummaryResponse> GetCheckoutSummaryAsync(int userId, int? addressId, CancellationToken cancellationToken = default)
    {
        var basket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        if (basket == null || !basket.Items.Any())
        {
            throw new InvalidOperationException(BasketConstants.BasketEmpty);
        }

        var basketResponse = MapToResponse(basket);
        var hasStockIssues = basketResponse.Items.Any(i => !i.IsAvailable || i.Quantity > i.Stock);

        ShippingAddress? address = null;
        if (addressId.HasValue)
        {
            address = await _shippingAddressRepository.GetByIdAsync(addressId.Value, cancellationToken);
            if (address != null && address.UserId != userId)
            {
                throw new UnauthorizedAccessException(ShippingAddressConstants.UnauthorizedAccess);
            }
        }

        var subTotal = basketResponse.TotalAmount;
        var shippingCost = 0m; // Free shipping for now

        return new CheckoutSummaryResponse
        {
            Basket = basketResponse,
            ShippingAddress = address != null ? MapAddressToResponse(address) : null,
            SubTotal = subTotal,
            ShippingCost = shippingCost,
            TotalAmount = subTotal + shippingCost,
            HasStockIssues = hasStockIssues
        };
    }

    private static BasketResponse MapToResponse(BasketEntity basket)
    {
        var items = basket.Items.Select(i => new BasketItemResponse
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? string.Empty,
            ProductImagePath = i.Product?.ImagePath,
            UnitPrice = i.UnitPrice,
            CurrentPrice = i.Product?.Price ?? i.UnitPrice,
            Quantity = i.Quantity,
            SubTotal = i.Quantity * (i.Product?.Price ?? i.UnitPrice),
            Stock = i.Product?.Stock ?? 0,
            IsAvailable = i.Product?.IsActive == true && (i.Product?.Stock ?? 0) > 0
        }).ToList();

        return new BasketResponse
        {
            Id = basket.Id,
            UserId = basket.UserId,
            Items = items,
            TotalAmount = items.Sum(i => i.SubTotal),
            TotalItems = items.Sum(i => i.Quantity),
            CreatedAt = basket.CreatedAt,
            UpdatedAt = basket.UpdatedAt
        };
    }

    private static ShippingAddressResponse MapAddressToResponse(Domain.Entities.ShippingAddress address)
    {
        return new ShippingAddressResponse
        {
            Id = address.Id,
            UserId = address.UserId,
            FullName = address.FullName,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt
        };
    }
}
