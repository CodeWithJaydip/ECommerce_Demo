using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.Features.User.DTOs.Requests;

/// <summary>
/// DTO for updating user information
/// </summary>
public record UpdateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; init; }

    [StringLength(20)]
    public string? PhoneNumber { get; init; }

    public bool? IsActive { get; init; }
}
