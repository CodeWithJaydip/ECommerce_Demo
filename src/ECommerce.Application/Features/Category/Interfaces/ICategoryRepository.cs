using ECommerce.Domain.Entities;
using CategoryEntity = ECommerce.Domain.Entities.Category;

namespace ECommerce.Application.Features.Category.Interfaces;

/// <summary>
/// Repository interface for Category entity
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<CategoryEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get category by name
    /// </summary>
    Task<CategoryEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active categories
    /// </summary>
    Task<List<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new category
    /// </summary>
    Task<CategoryEntity> CreateAsync(CategoryEntity category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing category
    /// </summary>
    Task<CategoryEntity> UpdateAsync(CategoryEntity category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft delete a category (set IsActive to false)
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category name exists (excluding the specified category ID)
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get category by ID for update (tracked entity)
    /// </summary>
    Task<CategoryEntity?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
}
