namespace ECommerce.Application.Features.Auth.DTOs.Responses;

/// <summary>
/// DTO for authentication response
/// </summary>
public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

/// <summary>
/// DTO for user information in authentication response
/// </summary>
public record UserDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public List<string> Roles { get; init; } = new();
}
