namespace ECommerce.Domain.Constants;

/// <summary>
/// Constants for Category entity validation and business rules
/// </summary>
public static class CategoryConstants
{
    /// <summary>
    /// Maximum length for category name
    /// </summary>
    public const int NameMaxLength = 100;

    /// <summary>
    /// Minimum length for category name
    /// </summary>
    public const int NameMinLength = 2;

    /// <summary>
    /// Maximum length for category description
    /// </summary>
    public const int DescriptionMaxLength = 255;

    /// <summary>
    /// Error message when category name is required
    /// </summary>
    public const string NameRequired = "Category name is required";

    /// <summary>
    /// Error message when category name length is invalid
    /// </summary>
    public const string NameLengthInvalid = "Category name must be between 2 and 100 characters";

    /// <summary>
    /// Error message when category description length is invalid
    /// </summary>
    public const string DescriptionLengthInvalid = "Category description must not exceed 255 characters";

    /// <summary>
    /// Error message when category not found
    /// </summary>
    public const string CategoryNotFound = "Category not found";

    /// <summary>
    /// Error message when category name already exists
    /// </summary>
    public const string CategoryNameExists = "A category with this name already exists";

    /// <summary>
    /// Error message when category cannot be deleted (has associated products)
    /// </summary>
    public const string CategoryCannotBeDeleted = "Category cannot be deleted because it has associated products";
}
