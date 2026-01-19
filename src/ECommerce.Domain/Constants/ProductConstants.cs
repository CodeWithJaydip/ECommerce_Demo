namespace ECommerce.Domain.Constants;

/// <summary>
/// Constants for Product entity validation and business rules
/// </summary>
public static class ProductConstants
{
    /// <summary>
    /// Maximum length for product name
    /// </summary>
    public const int NameMaxLength = 200;

    /// <summary>
    /// Minimum length for product name
    /// </summary>
    public const int NameMinLength = 2;

    /// <summary>
    /// Maximum length for product description
    /// </summary>
    public const int DescriptionMaxLength = 1000;

    /// <summary>
    /// Maximum length for SKU
    /// </summary>
    public const int SKUMaxLength = 100;

    /// <summary>
    /// Error message when product name is required
    /// </summary>
    public const string NameRequired = "Product name is required";

    /// <summary>
    /// Error message when product name length is invalid
    /// </summary>
    public const string NameLengthInvalid = "Product name must be between 2 and 200 characters";

    /// <summary>
    /// Error message when product description length is invalid
    /// </summary>
    public const string DescriptionLengthInvalid = "Product description must not exceed 1000 characters";

    /// <summary>
    /// Error message when product not found
    /// </summary>
    public const string ProductNotFound = "Product not found";

    /// <summary>
    /// Error message when product name already exists
    /// </summary>
    public const string ProductNameExists = "A product with this name already exists";

    /// <summary>
    /// Error message when price is invalid
    /// </summary>
    public const string PriceInvalid = "Price must be greater than 0";

    /// <summary>
    /// Error message when stock is invalid
    /// </summary>
    public const string StockInvalid = "Stock must be greater than or equal to 0";

    /// <summary>
    /// Error message when seller is not authorized
    /// </summary>
    public const string UnauthorizedSeller = "You are not authorized to perform this action on this product";
}
