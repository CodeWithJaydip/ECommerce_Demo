using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.User.DTOs.Requests;
using ECommerce.Application.Features.User.Enums;
using ECommerce.Domain.Entities;
using UserEntity = ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Features.Auth.Interfaces;

/// <summary>
/// Repository interface for User entity
/// </summary>
public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<Role>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated, filtered, and sorted list of users
    /// </summary>
    Task<PagedResult<UserEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        UserFilter? filter = null,
        UserSortBy? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove user role
    /// </summary>
    Task RemoveUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove all user roles
    /// </summary>
    Task RemoveAllUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update user status (IsActive) without updating other properties
    /// </summary>
    Task UpdateUserStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);
}
