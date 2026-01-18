using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Auth.Interfaces;
using ECommerce.Application.Features.User.DTOs.Requests;
using ECommerce.Application.Features.User.DTOs.Responses;
using ECommerce.Application.Features.User.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service implementation for User management
/// </summary>
public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedResult<UserResponse>> GetPagedAsync(UserListRequest request, CancellationToken cancellationToken = default)
    {
        var pagedResult = await _userRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.GetFilter(),
            request.GetSortByEnum(),
            request.SortDescending,
            cancellationToken);

        var userResponses = pagedResult.Items.Select(MapToResponse).ToList();

        return new PagedResult<UserResponse>(userResponses, pagedResult.Metadata.TotalCount, pagedResult.Metadata.PageNumber, pagedResult.Metadata.PageSize)
        {
            Metadata = pagedResult.Metadata
        };
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        // Check if email is being changed and if it's already taken
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException("Email is already taken by another user");
            }
        }

        // Update user properties
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email.ToLowerInvariant();
        }
        user.PhoneNumber = request.PhoneNumber;
        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload user with roles
        var updatedUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        return MapToResponse(updatedUser!);
    }

    public async Task<UserResponse> UpdateRolesAsync(int id, UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        // Validate all role IDs exist
        foreach (var roleId in request.RoleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found");
            }
        }

        // Remove all existing roles
        await _userRepository.RemoveAllUserRolesAsync(id, cancellationToken);

        // Add new roles
        foreach (var roleId in request.RoleIds)
        {
            await _userRepository.AddUserRoleAsync(id, roleId, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload user with roles
        var updatedUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        return MapToResponse(updatedUser!);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            EmailVerifiedAt = user.EmailVerifiedAt,
            LastLoginAt = user.LastLoginAt,
            LoginAttemptCount = user.LoginAttemptCount,
            LockedUntil = user.LockedUntil,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive,
            Roles = user.UserRoles
                .Where(ur => ur.IsActive)
                .Select(ur => ur.Role.Name)
                .Where(name => user.UserRoles.Any(ur => ur.Role.Name == name && ur.Role.IsActive))
                .ToList()
        };
    }
}
