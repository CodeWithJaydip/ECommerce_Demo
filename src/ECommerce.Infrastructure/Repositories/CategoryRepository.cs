using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Features.Category.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using CategoryEntity = ECommerce.Domain.Entities.Category;

namespace ECommerce.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Category entity
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);
    }

    public async Task<CategoryEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name && c.IsActive, cancellationToken);
    }

    public async Task<List<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryEntity> CreateAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        category.CreatedAt = DateTime.UtcNow;
        category.IsActive = true;
        await _context.Categories.AddAsync(category, cancellationToken);
        // No SaveChangesAsync here - handled by UnitOfWork
        return category;
    }

    public async Task<CategoryEntity> UpdateAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        category.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(category);
        // No SaveChangesAsync here - handled by UnitOfWork
        return await Task.FromResult(category);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

        if (category == null)
        {
            return false;
        }

        // Soft delete
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(category);
        // No SaveChangesAsync here - handled by UnitOfWork
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.Name == name && c.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CategoryEntity?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);
    }
}
