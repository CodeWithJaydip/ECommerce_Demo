using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Product entity for e-commerce products
/// </summary>
public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int SellerId { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    [StringLength(100)]
    public string? SKU { get; set; } // Stock Keeping Unit

    // Navigation properties
    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey(nameof(SellerId))]
    public virtual User Seller { get; set; } = null!;
}
