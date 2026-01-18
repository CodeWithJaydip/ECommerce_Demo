using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.User.DTOs.Requests;
using ECommerce.Application.Features.User.DTOs.Responses;

namespace ECommerce.Application.Features.User.Interfaces;

/// <summary>
/// Service interface for User management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get paginated list of users with filtering and sorting
    /// </summary>
    Task<PagedResult<UserResponse>> GetPagedAsync(UserListRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user information
    /// </summary>
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user roles
    /// </summary>
    Task<UserResponse> UpdateRolesAsync(int id, UpdateUserRolesRequest request, CancellationToken cancellationToken = default);
}
