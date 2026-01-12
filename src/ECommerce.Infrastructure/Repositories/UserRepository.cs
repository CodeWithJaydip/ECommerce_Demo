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
        // No SaveChangesAsync here - handled by UnitOfWork
        return user;
    }

    public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        // No SaveChangesAsync here - handled by UnitOfWork
        return Task.FromResult(user);
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
        // If userId or roleId is 0, this means entities haven't been saved yet
        // We'll need to get the tracked entities from the context
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);

        if (user == null || role == null)
        {
            // If entities aren't found by ID (maybe they're still tracked with Id=0),
            // try to get them from tracked entities
            user = _context.Users.Local.FirstOrDefault(u => u.Id == userId || (userId == 0 && u.Email != null));
            role = _context.Roles.Local.FirstOrDefault(r => r.Id == roleId || (roleId == 0 && r.Name != null));

            if (user == null || role == null)
            {
                throw new InvalidOperationException("User or Role not found. Entities must be tracked before adding UserRole.");
            }
        }

        var userRole = new UserRole
        {
            User = user,  // Use navigation property - EF Core will set UserId on SaveChanges
            Role = role,  // Use navigation property - EF Core will set RoleId on SaveChanges
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        // No SaveChangesAsync here - handled by UnitOfWork
    }
}
