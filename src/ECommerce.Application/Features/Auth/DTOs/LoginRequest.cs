using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECommerce.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for user login request
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
