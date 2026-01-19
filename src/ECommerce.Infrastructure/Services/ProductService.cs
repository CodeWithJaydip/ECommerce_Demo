using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Application.Features.Category.Interfaces;
using ECommerce.Application.Features.Product.DTOs.Requests;
using ECommerce.Application.Features.Product.DTOs.Responses;
using ECommerce.Application.Features.Product.Interfaces;
using ECommerce.Domain.Constants;
using ECommerce.Domain.Entities;
using ProductEntity = ECommerce.Domain.Entities.Product;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service implementation for Product operations
/// </summary>
public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IFileUploadService fileUploadService) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileUploadService _fileUploadService = fileUploadService;

    public async Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToResponse).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product == null ? null : MapToResponse(product);
    }

    public async Task<List<ProductResponse>> GetBySellerIdAsync(int sellerId, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetBySellerIdAsync(sellerId, cancellationToken);
        return products.Select(MapToResponse).ToList();
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, int? currentUserId, bool isSuperAdmin, CancellationToken cancellationToken = default)
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        // Determine seller ID
        int sellerId;
        if (isSuperAdmin)
        {
            // Super Admin can specify seller, or use current user if not specified
            sellerId = request.SellerId ?? currentUserId ?? throw new UnauthorizedAccessException("Seller ID is required");
        }
        else
        {
            // Seller can only create products for themselves
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("User ID is required");
            }
            sellerId = currentUserId.Value;
        }

        // Validate seller exists
        var seller = await _userRepository.GetByIdAsync(sellerId, cancellationToken);
        if (seller == null)
        {
            throw new KeyNotFoundException("Seller not found");
        }

        // Check if product name already exists
        if (await _productRepository.ExistsByNameAsync(request.Name, null, cancellationToken))
        {
            throw new InvalidOperationException(ProductConstants.ProductNameExists);
        }

        var product = new ProductEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            SellerId = sellerId,
            SKU = request.SKU?.Trim(),
            ImagePath = request.ImagePath,
            IsActive = true
        };

        product = await _productRepository.CreateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Refresh to get populated navigation properties
        product = await _productRepository.GetByIdAsync(product.Id, cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException("Failed to create product");
        }

        return MapToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request, int? currentUserId, bool isSuperAdmin, CancellationToken cancellationToken = default)
    {
        // Check if product exists
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException(ProductConstants.ProductNotFound);
        }

        // Authorization check: Seller can only update their own products
        if (!isSuperAdmin)
        {
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("User ID is required");
            }
            if (product.SellerId != currentUserId.Value)
            {
                throw new UnauthorizedAccessException(ProductConstants.UnauthorizedSeller);
            }
        }

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        // Check if product name already exists (excluding current product)
        if (await _productRepository.ExistsByNameAsync(request.Name, id, cancellationToken))
        {
            throw new InvalidOperationException(ProductConstants.ProductNameExists);
        }

        // Get tracked entity for update
        var trackedProduct = await _productRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (trackedProduct == null)
        {
            throw new KeyNotFoundException(ProductConstants.ProductNotFound);
        }

        // Delete old image if new image is provided
        if (!string.IsNullOrEmpty(request.ImagePath) && !string.IsNullOrEmpty(trackedProduct.ImagePath) && trackedProduct.ImagePath != request.ImagePath)
        {
            await _fileUploadService.DeleteImageAsync(trackedProduct.ImagePath);
        }

        // Update properties
        trackedProduct.Name = request.Name.Trim();
        trackedProduct.Description = request.Description?.Trim();
        trackedProduct.Price = request.Price;
        trackedProduct.Stock = request.Stock;
        trackedProduct.CategoryId = request.CategoryId;
        trackedProduct.SKU = request.SKU?.Trim();
        trackedProduct.ImagePath = request.ImagePath;

        await _productRepository.UpdateAsync(trackedProduct, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Refresh to get updated entity
        var updatedProduct = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (updatedProduct == null)
        {
            throw new InvalidOperationException("Failed to update product");
        }

        return MapToResponse(updatedProduct);
    }

    public async Task<bool> DeleteAsync(int id, int? currentUserId, bool isSuperAdmin, CancellationToken cancellationToken = default)
    {
        // Check if product exists
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException(ProductConstants.ProductNotFound);
        }

        // Authorization check: Seller can only delete their own products
        if (!isSuperAdmin)
        {
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("User ID is required");
            }
            if (product.SellerId != currentUserId.Value)
            {
                throw new UnauthorizedAccessException(ProductConstants.UnauthorizedSeller);
            }
        }

        var deleted = await _productRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException(ProductConstants.ProductNotFound);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Delete associated image file
        if (!string.IsNullOrEmpty(product.ImagePath))
        {
            await _fileUploadService.DeleteImageAsync(product.ImagePath);
        }

        return true;
    }

    private static ProductResponse MapToResponse(ProductEntity product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            SellerId = product.SellerId,
            SellerName = product.Seller?.FullName ?? string.Empty,
            ImagePath = product.ImagePath,
            SKU = product.SKU,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            IsActive = product.IsActive
        };
    }
}
