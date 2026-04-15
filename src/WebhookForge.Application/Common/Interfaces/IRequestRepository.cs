using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Extends the generic repository with incoming-request-specific queries.
/// Source: Application layer — implemented in Infrastructure/Repositories/RequestRepository.cs.
/// </summary>
public interface IRequestRepository : IRepository<IncomingRequest>
{
    /// <summary>
    /// Returns a page of requests for the given endpoint, ordered by ReceivedAt descending.
    /// Used by the paginated request history API.
    /// </summary>
    Task<IReadOnlyList<IncomingRequest>> GetPagedByEndpointIdAsync(
        Guid endpointId, int page, int pageSize, CancellationToken ct = default);

    /// <summary>Returns total count of captured requests for an endpoint. Used alongside paging.</summary>
    Task<int> GetCountByEndpointIdAsync(Guid endpointId, CancellationToken ct = default);

    /// <summary>
    /// Bulk-deletes requests older than <paramref name="cutoffDate"/> for a specific endpoint.
    /// Returns the number of rows deleted.
    /// </summary>
    Task<int> DeleteOlderThanAsync(Guid endpointId, DateTime cutoffDate, CancellationToken ct = default);
}
