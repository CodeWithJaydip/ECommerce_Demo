using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.User.Enums;

namespace ECommerce.Application.Features.User.DTOs.Requests;

/// <summary>
/// Request model for listing users with pagination, filtering, and sorting
/// All properties can be bound directly from query string parameters
/// </summary>
public record UserListRequest : PagedRequest
{
    // Filter properties (flattened for query string binding)
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

    /// <summary>
    /// Field to sort by (whitelisted enum) - accepts string from query and converts to enum
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (true for descending, false for ascending)
    /// </summary>
    public bool SortDescending { get; set; } = false;

    /// <summary>
    /// Convert to UserFilter object for service layer
    /// </summary>
    public UserFilter? GetFilter()
    {
        if (string.IsNullOrWhiteSpace(FirstName) &&
            string.IsNullOrWhiteSpace(LastName) &&
            string.IsNullOrWhiteSpace(Email) &&
            string.IsNullOrWhiteSpace(PhoneNumber) &&
            !IsActive.HasValue &&
            string.IsNullOrWhiteSpace(RoleName) &&
            !IsLocked.HasValue)
        {
            return null;
        }

        return new UserFilter
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            IsActive = IsActive,
            RoleName = RoleName,
            IsLocked = IsLocked
        };
    }

    /// <summary>
    /// Get parsed SortBy enum value
    /// </summary>
    public UserSortBy? GetSortByEnum()
    {
        if (string.IsNullOrWhiteSpace(SortBy))
        {
            return null;
        }

        return Enum.TryParse<UserSortBy>(SortBy, ignoreCase: true, out var sortByEnum)
            ? sortByEnum
            : null;
    }
}
