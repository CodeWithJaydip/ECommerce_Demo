namespace ECommerce.Application.Features.Product.DTOs.Requests;

/// <summary>
/// Request model for catalog search, filtering, sorting, and pagination
/// </summary>
public class CatalogRequest
{
    /// <summary>
    /// Search term to match against product name or description
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by category ID
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Minimum price filter
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price filter
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Filter to show only in-stock products
    /// </summary>
    public bool? InStock { get; set; }

    /// <summary>
    /// Sort field: name, price, createdAt (default: createdAt)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction: true = descending, false = ascending (default: true)
    /// </summary>
    public bool SortDescending { get; set; } = true;

    /// <summary>
    /// Page number (1-based, default: 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page (default: 12)
    /// </summary>
    public int PageSize { get; set; } = 12;
}
