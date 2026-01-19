using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Category.DTOs.Requests;
using ECommerce.Application.Features.Category.DTOs.Responses;
using ECommerce.Application.Features.Category.Interfaces;
using ECommerce.Infrastructure.Services;

namespace ECommerce.Api.Controllers;

/// <summary>
/// Controller for Category CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService, IFileUploadService fileUploadService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IFileUploadService _fileUploadService = fileUploadService;

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    [Authorize] // Requires authentication - any authenticated user (Buyer, Seller, Super Admin) can access
    public async Task<ActionResult<ApiResponse<List<CategoryResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(categories, "Categories retrieved successfully"));
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize] // Requires authentication - any authenticated user (Buyer, Seller, Super Admin) can access
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return NotFound(ApiResponse<CategoryResponse>.ErrorResponse("Category not found"));
        }
        return Ok(ApiResponse<CategoryResponse>.SuccessResponse(category, "Category retrieved successfully"));
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Super Admin")] // Only Super Admin can create categories
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> Create([FromForm] string name, [FromForm] string? description, [FromForm] IFormFile? image, CancellationToken cancellationToken)
    {
        string? imagePath = null;

        // Upload image if provided
        if (image != null && image.Length > 0)
        {
            imagePath = await _fileUploadService.UploadImageAsync(image, "categories");
        }

        var request = new CreateCategoryRequest
        {
            Name = name,
            Description = description,
            ImagePath = imagePath
        };

        var category = await _categoryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            ApiResponse<CategoryResponse>.SuccessResponse(category, "Category created successfully"));
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Super Admin")] // Only Super Admin can update categories
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> Update(int id, [FromForm] string name, [FromForm] string? description, [FromForm] IFormFile? image, [FromForm] bool? removeImage, CancellationToken cancellationToken)
    {
        string? imagePath = null;

        // Get existing category to check current image
        var existingCategory = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (existingCategory == null)
        {
            return NotFound(ApiResponse<CategoryResponse>.ErrorResponse("Category not found"));
        }

        // Handle image upload/removal
        if (removeImage == true)
        {
            // Delete existing image
            if (!string.IsNullOrEmpty(existingCategory.ImagePath))
            {
                await _fileUploadService.DeleteImageAsync(existingCategory.ImagePath);
            }
            imagePath = null;
        }
        else if (image != null && image.Length > 0)
        {
            // Upload new image
            imagePath = await _fileUploadService.UploadImageAsync(image, "categories");
            // Delete old image if exists
            if (!string.IsNullOrEmpty(existingCategory.ImagePath))
            {
                await _fileUploadService.DeleteImageAsync(existingCategory.ImagePath);
            }
        }
        else
        {
            // Keep existing image
            imagePath = existingCategory.ImagePath;
        }

        var request = new UpdateCategoryRequest
        {
            Name = name,
            Description = description,
            ImagePath = imagePath
        };

        var category = await _categoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<CategoryResponse>.SuccessResponse(category, "Category updated successfully"));
    }

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin")] // Only Super Admin can delete categories
    public async Task<ActionResult<ApiResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse.ErrorResponse("Category not found"));
        }
        return Ok(ApiResponse.SuccessResponse("Category deleted successfully"));
    }
}
