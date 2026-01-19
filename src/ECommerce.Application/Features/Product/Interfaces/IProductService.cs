using ECommerce.Application.Features.Product.DTOs.Requests;
using ECommerce.Application.Features.Product.DTOs.Responses;

namespace ECommerce.Application.Features.Product.Interfaces;

/// <summary>
/// Service interface for Product operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get all products (public access)
    /// </summary>
    Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by ID (public access)
    /// </summary>
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get products by seller ID
    /// </summary>
    Task<List<ProductResponse>> GetBySellerIdAsync(int sellerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new product (Super Admin or Seller)
    /// </summary>
    Task<ProductResponse> CreateAsync(CreateProductRequest request, int? currentUserId, bool isSuperAdmin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing product (Super Admin or Seller - only their own)
    /// </summary>
    Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request, int? currentUserId, bool isSuperAdmin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a product (Super Admin or Seller - only their own)
    /// </summary>
    Task<bool> DeleteAsync(int id, int? currentUserId, bool isSuperAdmin, CancellationToken cancellationToken = default);
}
