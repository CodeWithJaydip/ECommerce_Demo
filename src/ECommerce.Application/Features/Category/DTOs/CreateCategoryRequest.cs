using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Constants;

namespace ECommerce.Application.Features.Category.DTOs;

/// <summary>
/// DTO for creating a new category
/// </summary>
public class CreateCategoryRequest
{
    [Required(ErrorMessage = CategoryConstants.NameRequired)]
    [StringLength(CategoryConstants.NameMaxLength, MinimumLength = CategoryConstants.NameMinLength, ErrorMessage = CategoryConstants.NameLengthInvalid)]
    public string Name { get; set; } = string.Empty;

    [StringLength(CategoryConstants.DescriptionMaxLength, ErrorMessage = CategoryConstants.DescriptionLengthInvalid)]
    public string? Description { get; set; }
}
