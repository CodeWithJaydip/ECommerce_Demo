using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Role entity
/// </summary>
public class Role : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
