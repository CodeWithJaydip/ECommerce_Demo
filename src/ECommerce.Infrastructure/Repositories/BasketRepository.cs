using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Features.Basket.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using BasketEntity = ECommerce.Domain.Entities.Basket;

namespace ECommerce.Infrastructure.Repositories;

public class BasketRepository(ApplicationDbContext context) : IBasketRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<BasketEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Baskets
            .AsNoTracking()
            .Include(b => b.Items.Where(i => i.IsActive))
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive, cancellationToken);
    }

    public async Task<BasketEntity?> GetByUserIdForUpdateAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Baskets
            .Include(b => b.Items.Where(i => i.IsActive))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive, cancellationToken);
    }

    public async Task<BasketEntity> CreateAsync(BasketEntity basket, CancellationToken cancellationToken = default)
    {
        basket.CreatedAt = DateTime.UtcNow;
        basket.IsActive = true;
        await _context.Baskets.AddAsync(basket, cancellationToken);
        return basket;
    }

    public async Task<BasketItem?> GetItemAsync(int basketId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.BasketItems
            .AsNoTracking()
            .Include(bi => bi.Product)
            .FirstOrDefaultAsync(bi => bi.BasketId == basketId && bi.ProductId == productId && bi.IsActive, cancellationToken);
    }

    public async Task<BasketItem?> GetItemForUpdateAsync(int basketId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.BasketItems
            .Include(bi => bi.Product)
            .FirstOrDefaultAsync(bi => bi.BasketId == basketId && bi.ProductId == productId && bi.IsActive, cancellationToken);
    }

    public async Task<BasketItem?> GetInactiveItemForUpdateAsync(int basketId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.BasketItems
            .Include(bi => bi.Product)
            .FirstOrDefaultAsync(bi => bi.BasketId == basketId && bi.ProductId == productId && !bi.IsActive, cancellationToken);
    }

    public async Task<BasketItem> AddItemAsync(BasketItem item, CancellationToken cancellationToken = default)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.IsActive = true;
        await _context.BasketItems.AddAsync(item, cancellationToken);
        return item;
    }

    public async Task RemoveItemAsync(BasketItem item, CancellationToken cancellationToken = default)
    {
        item.IsActive = false;
        item.UpdatedAt = DateTime.UtcNow;
        _context.BasketItems.Update(item);
        await Task.CompletedTask;
    }

    public async Task ClearItemsAsync(int basketId, CancellationToken cancellationToken = default)
    {
        var items = await _context.BasketItems
            .Where(bi => bi.BasketId == basketId && bi.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<int> GetItemCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        var basket = await _context.Baskets
            .AsNoTracking()
            .Include(b => b.Items.Where(i => i.IsActive))
            .FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive, cancellationToken);

        return basket?.Items.Sum(i => i.Quantity) ?? 0;
    }
}
