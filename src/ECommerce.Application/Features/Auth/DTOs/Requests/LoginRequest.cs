using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECommerce.Application.Features.Auth.DTOs.Requests;

/// <summary>
/// DTO for user login request
/// </summary>
public record LoginRequest
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;
}
