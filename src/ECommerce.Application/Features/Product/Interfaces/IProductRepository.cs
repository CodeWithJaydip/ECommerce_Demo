using ECommerce.Domain.Entities;
using ProductEntity = ECommerce.Domain.Entities.Product;

namespace ECommerce.Application.Features.Product.Interfaces;

/// <summary>
/// Repository interface for Product data access
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Get all active products
    /// </summary>
    Task<List<ProductEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by ID
    /// </summary>
    Task<ProductEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by ID for update (with tracking)
    /// </summary>
    Task<ProductEntity?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products by seller ID
    /// </summary>
    Task<List<ProductEntity>> GetBySellerIdAsync(int sellerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new product
    /// </summary>
    Task<ProductEntity> CreateAsync(ProductEntity product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing product
    /// </summary>
    Task UpdateAsync(ProductEntity product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft delete a product
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if product name already exists
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, int? excludeProductId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if product belongs to seller
    /// </summary>
    Task<bool> BelongsToSellerAsync(int productId, int sellerId, CancellationToken cancellationToken = default);
}
