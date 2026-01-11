using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.CreatedAt = DateTime.UtcNow;
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<List<Role>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
