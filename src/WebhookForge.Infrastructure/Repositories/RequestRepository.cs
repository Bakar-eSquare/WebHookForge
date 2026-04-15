using Microsoft.EntityFrameworkCore;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// Request-specific queries on top of the generic repository.
/// Source: Infrastructure layer — implements IRequestRepository (Application).
/// </summary>
public class RequestRepository : Repository<IncomingRequest>, IRequestRepository
{
    public RequestRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IncomingRequest>> GetPagedByEndpointIdAsync(
        Guid endpointId, int page, int pageSize, CancellationToken ct = default)
        => await _set
            .Where(r => r.EndpointId == endpointId)
            .OrderByDescending(r => r.ReceivedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<int> GetCountByEndpointIdAsync(Guid endpointId, CancellationToken ct = default)
        => await _set.CountAsync(r => r.EndpointId == endpointId, ct);

    /// <inheritdoc/>
    /// Uses EF Core ExecuteDeleteAsync for a single-trip bulk delete without loading entities into memory.
    public async Task<int> DeleteOlderThanAsync(Guid endpointId, DateTime cutoffDate, CancellationToken ct = default)
        => await _set
            .Where(r => r.EndpointId == endpointId && r.ReceivedAt < cutoffDate)
            .ExecuteDeleteAsync(ct);
}
