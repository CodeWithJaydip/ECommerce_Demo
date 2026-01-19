using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.User.DTOs.Requests;
using ECommerce.Application.Features.User.DTOs.Responses;
using ECommerce.Application.Features.User.Interfaces;

namespace ECommerce.Api.Controllers;

/// <summary>
/// Controller for User management operations (Super Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Super Admin")] // All endpoints require Super Admin role
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Get paginated list of users with filtering and sorting
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserResponse>>>> GetAll(
        [FromQuery] UserListRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<UserResponse>>.SuccessResponse(result, "Users retrieved successfully"));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(ApiResponse<UserResponse>.ErrorResponse("User not found"));
        }
        return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "User retrieved successfully"));
    }

    /// <summary>
    /// Update user information
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "User updated successfully"));
    }

    /// <summary>
    /// Update user roles
    /// </summary>
    [HttpPut("{id}/roles")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateRoles(
        int id,
        [FromBody] UpdateUserRolesRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateRolesAsync(id, request, cancellationToken);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "User roles updated successfully"));
    }

    /// <summary>
    /// Delete (deactivate) user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var user = await _userService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "User deleted successfully"));
    }
}
