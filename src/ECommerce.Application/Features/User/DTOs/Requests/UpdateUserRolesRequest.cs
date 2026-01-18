using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.Features.User.DTOs.Requests;

/// <summary>
/// DTO for updating user roles
/// </summary>
public record UpdateUserRolesRequest
{
    /// <summary>
    /// List of role IDs to assign to the user
    /// </summary>
    [Required]
    public List<int> RoleIds { get; init; } = new();
}
