using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECommerce.Application.Features.Auth.DTOs;

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [JsonPropertyName("confirmPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(20)]
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }
}
