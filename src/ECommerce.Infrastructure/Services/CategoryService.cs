using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Application.Features.Category.DTOs.Requests;
using ECommerce.Application.Features.Category.DTOs.Responses;
using ECommerce.Application.Features.Category.Interfaces;
using ECommerce.Domain.Constants;
using ECommerce.Domain.Entities;
using CategoryEntity = ECommerce.Domain.Entities.Category;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service implementation for Category operations
/// </summary>
public class CategoryService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IFileUploadService fileUploadService) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileUploadService _fileUploadService = fileUploadService;

    public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category == null ? null : MapToResponse(category);
    }

    public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        // Check if category name already exists
        if (await _categoryRepository.ExistsByNameAsync(request.Name, null, cancellationToken))
        {
            throw new InvalidOperationException(CategoryConstants.CategoryNameExists);
        }

        var category = new CategoryEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            ImagePath = request.ImagePath,
            IsActive = true
        };

        category = await _categoryRepository.CreateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Refresh to get the populated ID
        category = await _categoryRepository.GetByIdAsync(category.Id, cancellationToken);
        if (category == null)
        {
            throw new InvalidOperationException("Failed to create category");
        }

        return MapToResponse(category);
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        // Check if category exists
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException(CategoryConstants.CategoryNotFound);
        }

        // Check if category name already exists (excluding current category)
        if (await _categoryRepository.ExistsByNameAsync(request.Name, id, cancellationToken))
        {
            throw new InvalidOperationException(CategoryConstants.CategoryNameExists);
        }

        // Get tracked entity for update
        var trackedCategory = await _categoryRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (trackedCategory == null)
        {
            throw new KeyNotFoundException(CategoryConstants.CategoryNotFound);
        }

        // Delete old image if new image is provided
        if (!string.IsNullOrEmpty(request.ImagePath) && !string.IsNullOrEmpty(trackedCategory.ImagePath) && trackedCategory.ImagePath != request.ImagePath)
        {
            await _fileUploadService.DeleteImageAsync(trackedCategory.ImagePath);
        }

        // Update properties
        trackedCategory.Name = request.Name.Trim();
        trackedCategory.Description = request.Description?.Trim();
        trackedCategory.ImagePath = request.ImagePath;

        await _categoryRepository.UpdateAsync(trackedCategory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Refresh to get updated entity
        var updatedCategory = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (updatedCategory == null)
        {
            throw new InvalidOperationException("Failed to update category");
        }

        return MapToResponse(updatedCategory);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Get category to delete image
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException(CategoryConstants.CategoryNotFound);
        }

        var deleted = await _categoryRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException(CategoryConstants.CategoryNotFound);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Delete associated image file
        if (!string.IsNullOrEmpty(category.ImagePath))
        {
            await _fileUploadService.DeleteImageAsync(category.ImagePath);
        }

        return true;
    }

    private static CategoryResponse MapToResponse(CategoryEntity category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImagePath = category.ImagePath,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            IsActive = category.IsActive
        };
    }
}
