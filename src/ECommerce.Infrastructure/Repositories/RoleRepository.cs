using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Role entity
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name && r.IsActive, cancellationToken);
    }

    public async Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        role.CreatedAt = DateTime.UtcNow;
        await _context.Roles.AddAsync(role, cancellationToken);
        // No SaveChangesAsync here - handled by UnitOfWork
        return role;
    }

    public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);
    }
}
