using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Application.Features.User.DTOs.Requests;
using ECommerce.Application.Features.User.Enums;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using System.Linq.Expressions;

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

    public async Task<PagedResult<User>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        UserFilter? filter = null,
        UserSortBy? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        // Build query with AsNoTracking for read-only operations
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .AsQueryable();

        // Apply filters (strongly typed, no magic strings)
        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.FirstName))
            {
                query = query.Where(u => u.FirstName.Contains(filter.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(filter.LastName))
            {
                query = query.Where(u => u.LastName.Contains(filter.LastName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                query = query.Where(u => u.Email.Contains(filter.Email));
            }

            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(filter.PhoneNumber));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.RoleName))
            {
                // Use Contains for partial matching (case-insensitive for SQL Server)
                query = query.Where(u => u.UserRoles.Any(ur => 
                    ur.Role.Name != null && 
                    ur.Role.Name.Contains(filter.RoleName) && 
                    ur.IsActive && 
                    ur.Role.IsActive));
            }

            if (filter.IsLocked.HasValue)
            {
                if (filter.IsLocked.Value)
                {
                    query = query.Where(u => u.LockedUntil.HasValue && u.LockedUntil.Value > DateTime.UtcNow);
                }
                else
                {
                    query = query.Where(u => !u.LockedUntil.HasValue || u.LockedUntil.Value <= DateTime.UtcNow);
                }
            }
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting (whitelisted enum, no magic strings)
        if (sortBy.HasValue)
        {
            query = sortBy.Value switch
            {
                UserSortBy.FirstName => sortDescending
                    ? query.OrderByDescending(u => u.FirstName)
                    : query.OrderBy(u => u.FirstName),
                UserSortBy.LastName => sortDescending
                    ? query.OrderByDescending(u => u.LastName)
                    : query.OrderBy(u => u.LastName),
                UserSortBy.Email => sortDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                UserSortBy.CreatedAt => sortDescending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                UserSortBy.LastLoginAt => sortDescending
                    ? query.OrderByDescending(u => u.LastLoginAt ?? DateTime.MinValue)
                    : query.OrderBy(u => u.LastLoginAt ?? DateTime.MinValue),
                _ => query.OrderBy(u => u.Id)
            };
        }
        else
        {
            // Default sort by ID
            query = query.OrderBy(u => u.Id);
        }

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task RemoveUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive, cancellationToken);

        if (userRole != null)
        {
            userRole.IsActive = false;
            userRole.UpdatedAt = DateTime.UtcNow;
            _context.UserRoles.Update(userRole);
        }
        // No SaveChangesAsync here - handled by UnitOfWork
    }

    public async Task RemoveAllUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var userRole in userRoles)
        {
            userRole.IsActive = false;
            userRole.UpdatedAt = DateTime.UtcNow;
        }

        if (userRoles.Any())
        {
            _context.UserRoles.UpdateRange(userRoles);
        }
        // No SaveChangesAsync here - handled by UnitOfWork
    }

    public async Task UpdateUserStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default)
    {
        // Use FindAsync to get tracked entity, or attach if not tracked
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        
        if (user == null)
        {
            // If not found, try to get it (might be filtered by IsActive in GetByIdAsync)
            user = await _context.Users
                .IgnoreQueryFilters() // Bypass any query filters (like IsActive)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }
        }

        // Mark only IsActive and UpdatedAt as modified to avoid validation issues
        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        
        // Explicitly mark only these properties as modified
        _context.Entry(user).Property(u => u.IsActive).IsModified = true;
        _context.Entry(user).Property(u => u.UpdatedAt).IsModified = true;
        
        // No SaveChangesAsync here - handled by UnitOfWork
    }
}
