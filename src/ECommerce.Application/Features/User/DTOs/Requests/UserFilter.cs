namespace ECommerce.Application.Features.User.DTOs.Requests;

/// <summary>
/// Strongly typed filter object for user filtering
/// </summary>
public record UserFilter
{
    /// <summary>
    /// Filter by first name (partial match)
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Filter by last name (partial match)
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Filter by email (partial match)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Filter by phone number (partial match)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter by role name (partial match)
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// Filter by locked status (users that are currently locked)
    /// </summary>
    public bool? IsLocked { get; set; }
}
