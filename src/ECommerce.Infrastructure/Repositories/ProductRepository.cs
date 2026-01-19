using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Features.Product.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ProductEntity = ECommerce.Domain.Entities.Product;

namespace ECommerce.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Product entity
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<ProductEntity?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<List<ProductEntity>> GetBySellerIdAsync(int sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Where(p => p.SellerId == sellerId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductEntity> CreateAsync(ProductEntity product, CancellationToken cancellationToken = default)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;
        await _context.Products.AddAsync(product, cancellationToken);
        // No SaveChangesAsync here - handled by UnitOfWork
        return product;
    }

    public async Task UpdateAsync(ProductEntity product, CancellationToken cancellationToken = default)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        // No SaveChangesAsync here - handled by UnitOfWork
        await Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

        if (product == null)
        {
            return false;
        }

        // Soft delete
        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        // No SaveChangesAsync here - handled by UnitOfWork
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.Name == name && p.IsActive);

        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> BelongsToSellerAsync(int productId, int sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == productId && p.SellerId == sellerId && p.IsActive, cancellationToken);
    }
}
