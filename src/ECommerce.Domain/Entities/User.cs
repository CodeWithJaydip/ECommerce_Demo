using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

/// <summary>
/// User entity for custom user management
/// </summary>
public class User : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int LoginAttemptCount { get; set; }

    public DateTime? LockedUntil { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}
