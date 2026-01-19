using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Constants;

namespace ECommerce.Application.Features.Category.DTOs.Requests;

/// <summary>
/// DTO for creating a new category
/// </summary>
public record CreateCategoryRequest
{
    [Required(ErrorMessage = CategoryConstants.NameRequired)]
    [StringLength(CategoryConstants.NameMaxLength, MinimumLength = CategoryConstants.NameMinLength, ErrorMessage = CategoryConstants.NameLengthInvalid)]
    public string Name { get; init; } = string.Empty;

    [StringLength(CategoryConstants.DescriptionMaxLength, ErrorMessage = CategoryConstants.DescriptionLengthInvalid)]
    public string? Description { get; init; }

    public string? ImagePath { get; init; }
}
