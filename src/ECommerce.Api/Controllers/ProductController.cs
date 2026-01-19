using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Product.DTOs.Requests;
using ECommerce.Application.Features.Product.DTOs.Responses;
using ECommerce.Application.Features.Product.Interfaces;
using ECommerce.Infrastructure.Services;

namespace ECommerce.Api.Controllers;

/// <summary>
/// Controller for Product CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService, IFileUploadService fileUploadService) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IFileUploadService _fileUploadService = fileUploadService;

    /// <summary>
    /// Get all products (public access)
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Public access - no authentication required
    public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<ProductResponse>>.SuccessResponse(products, "Products retrieved successfully"));
    }

    /// <summary>
    /// Get product by ID (public access)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous] // Public access - no authentication required
    public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductResponse>.ErrorResponse("Product not found"));
        }
        return Ok(ApiResponse<ProductResponse>.SuccessResponse(product, "Product retrieved successfully"));
    }

    /// <summary>
    /// Create a new product (Super Admin or Seller)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Super Admin,Seller")] // Super Admin or Seller can create products
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Create(
        [FromForm] string name,
        [FromForm] string? description,
        [FromForm] decimal price,
        [FromForm] int stock,
        [FromForm] int categoryId,
        [FromForm] int? sellerId,
        [FromForm] string? sku,
        [FromForm] IFormFile? image,
        CancellationToken cancellationToken)
    {
        // Get current user info from JWT token
        var currentUserId = GetCurrentUserId();
        var isSuperAdmin = User.IsInRole("Super Admin");

        string? imagePath = null;

        // Upload image if provided
        if (image != null && image.Length > 0)
        {
            imagePath = await _fileUploadService.UploadImageAsync(image, "products");
        }

        var request = new CreateProductRequest
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CategoryId = categoryId,
            SellerId = sellerId, // Only used if Super Admin
            SKU = sku,
            ImagePath = imagePath
        };

        var product = await _productService.CreateAsync(request, currentUserId, isSuperAdmin, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ApiResponse<ProductResponse>.SuccessResponse(product, "Product created successfully"));
    }

    /// <summary>
    /// Update an existing product (Super Admin or Seller - only their own)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Super Admin,Seller")] // Super Admin or Seller can update products
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Update(
        int id,
        [FromForm] string name,
        [FromForm] string? description,
        [FromForm] decimal price,
        [FromForm] int stock,
        [FromForm] int categoryId,
        [FromForm] string? sku,
        [FromForm] IFormFile? image,
        [FromForm] bool? removeImage,
        CancellationToken cancellationToken)
    {
        // Get current user info from JWT token
        var currentUserId = GetCurrentUserId();
        var isSuperAdmin = User.IsInRole("Super Admin");

        // Get existing product to check current image
        var existingProduct = await _productService.GetByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            return NotFound(ApiResponse<ProductResponse>.ErrorResponse("Product not found"));
        }

        string? imagePath = null;

        // Handle image upload/removal
        if (removeImage == true)
        {
            // Delete existing image
            if (!string.IsNullOrEmpty(existingProduct.ImagePath))
            {
                await _fileUploadService.DeleteImageAsync(existingProduct.ImagePath);
            }
            imagePath = null;
        }
        else if (image != null && image.Length > 0)
        {
            // Upload new image
            imagePath = await _fileUploadService.UploadImageAsync(image, "products");
            // Delete old image if exists
            if (!string.IsNullOrEmpty(existingProduct.ImagePath))
            {
                await _fileUploadService.DeleteImageAsync(existingProduct.ImagePath);
            }
        }
        else
        {
            // Keep existing image
            imagePath = existingProduct.ImagePath;
        }

        var request = new UpdateProductRequest
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CategoryId = categoryId,
            SKU = sku,
            ImagePath = imagePath
        };

        var product = await _productService.UpdateAsync(id, request, currentUserId, isSuperAdmin, cancellationToken);
        return Ok(ApiResponse<ProductResponse>.SuccessResponse(product, "Product updated successfully"));
    }

    /// <summary>
    /// Delete a product (Super Admin or Seller - only their own)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin,Seller")] // Super Admin or Seller can delete products
    public async Task<ActionResult<ApiResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        // Get current user info from JWT token
        var currentUserId = GetCurrentUserId();
        var isSuperAdmin = User.IsInRole("Super Admin");

        var deleted = await _productService.DeleteAsync(id, currentUserId, isSuperAdmin, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse.ErrorResponse("Product not found"));
        }
        return Ok(ApiResponse.SuccessResponse("Product deleted successfully"));
    }

    /// <summary>
    /// Get current user ID from JWT token claims
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
