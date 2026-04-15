namespace WebhookForge.Application.Common.Models;

/// <summary>
/// Wraps a page of items together with paging metadata.
/// Returned by list endpoints that support pagination.
/// </summary>
public class PagedResult<T>
{
    /// <summary>Items on the current page.</summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>Total number of matching items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>Current page number (1-based).</summary>
    public int Page { get; init; }

    /// <summary>Maximum items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of pages.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>True if a previous page exists.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>True if a next page exists.</summary>
    public bool HasNextPage => Page < TotalPages;
}
