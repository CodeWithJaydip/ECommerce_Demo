using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Constants;

namespace ECommerce.Application.Features.Product.DTOs.Requests;

/// <summary>
/// DTO for updating an existing product
/// </summary>
public record UpdateProductRequest
{
    [Required(ErrorMessage = ProductConstants.NameRequired)]
    [StringLength(ProductConstants.NameMaxLength, MinimumLength = ProductConstants.NameMinLength, ErrorMessage = ProductConstants.NameLengthInvalid)]
    public string Name { get; init; } = string.Empty;

    [StringLength(ProductConstants.DescriptionMaxLength, ErrorMessage = ProductConstants.DescriptionLengthInvalid)]
    public string? Description { get; init; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = ProductConstants.PriceInvalid)]
    public decimal Price { get; init; }

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = ProductConstants.StockInvalid)]
    public int Stock { get; init; }

    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; init; }

    [StringLength(ProductConstants.SKUMaxLength)]
    public string? SKU { get; init; }

    public string? ImagePath { get; init; }
}
