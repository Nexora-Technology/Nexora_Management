namespace Nexora.Management.Application.Common;

/// <summary>
/// Paged response wrapper for list data
/// </summary>
/// <typeparam name="T">Type of items in the data collection</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Collection of data items for current page
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indicates whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a paged response
    /// </summary>
    public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
