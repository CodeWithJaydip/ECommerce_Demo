namespace ECommerce.Application.Features.User.Enums;

/// <summary>
/// Whitelisted fields for user sorting
/// </summary>
public enum UserSortBy
{
    /// <summary>
    /// Sort by first name
    /// </summary>
    FirstName,

    /// <summary>
    /// Sort by last name
    /// </summary>
    LastName,

    /// <summary>
    /// Sort by email
    /// </summary>
    Email,

    /// <summary>
    /// Sort by creation date
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort by last login date
    /// </summary>
    LastLoginAt
}
