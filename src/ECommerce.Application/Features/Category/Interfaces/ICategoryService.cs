using ECommerce.Application.Features.Category.DTOs;

namespace ECommerce.Application.Features.Category.Interfaces;

/// <summary>
/// Service interface for Category operations
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all categories
    /// </summary>
    Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new category
    /// </summary>
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing category
    /// </summary>
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
