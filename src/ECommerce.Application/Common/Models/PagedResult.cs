namespace ECommerce.Application.Common.Models;

/// <summary>
/// Generic paginated result with metadata
/// </summary>
public record PagedResult<T>(List<T> Items, PagedMetadata Metadata)
{
    public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        : this(items, new PagedMetadata(totalCount, pageNumber, pageSize))
    {
    }
}

/// <summary>
/// Pagination metadata
/// </summary>
public record PagedMetadata(int TotalCount, int PageNumber, int PageSize)
{
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; init; } = (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPrevious { get; init; } = PageNumber > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNext { get; init; } = PageNumber < (int)Math.Ceiling(TotalCount / (double)PageSize);
}
