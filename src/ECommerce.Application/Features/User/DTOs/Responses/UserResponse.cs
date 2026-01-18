namespace ECommerce.Application.Features.User.DTOs.Responses;

/// <summary>
/// DTO for user response
/// </summary>
public record UserResponse
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTime? EmailVerifiedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public int LoginAttemptCount { get; init; }
    public DateTime? LockedUntil { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
    public List<string> Roles { get; init; } = new();
}
