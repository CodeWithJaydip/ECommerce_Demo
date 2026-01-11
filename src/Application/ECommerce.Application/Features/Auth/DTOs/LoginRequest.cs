using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for user login request
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
