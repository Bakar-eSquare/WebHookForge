using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// Endpoint-specific queries on top of the generic repository.
///
/// Caching strategy — hot path only:
///   GetByTokenAsync is called on every incoming webhook hit (public, unauthenticated).
///   We cache the token → endpoint mapping for <see cref="TokenCacheTtl"/> to avoid
///   a DB round-trip on every request.
///
/// Invalidation:
///   Override UpdateAsync and DeleteAsync to evict by the old token.
///   A secondary key (ep:id:{id}) stores the currently cached token so that
///   token-regeneration scenarios (where the token on the entity has already changed
///   before UpdateAsync is called) are handled correctly.
/// </summary>
public class EndpointRepository : Repository<WebhookEndpoint>, IEndpointRepository
{
    private static readonly TimeSpan TokenCacheTtl = TimeSpan.FromMinutes(5);

    // Cache key helpers — centralised so a typo never causes a silent miss
    private static string TokenKey(string token) => $"ep:tok:{token}";
    private static string IdKey(Guid id)         => $"ep:id:{id}";

    private readonly IMemoryCache _cache;

    public EndpointRepository(ApplicationDbContext context, IMemoryCache cache) : base(context)
        => _cache = cache;

    /// <inheritdoc/>
    /// Hot path: token lookup is cached to avoid a DB hit on every incoming webhook.
    public async Task<WebhookEndpoint?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(TokenKey(token), out WebhookEndpoint? cached))
            return cached;

        var endpoint = await _set.FirstOrDefaultAsync(e => e.Token == token, ct);

        if (endpoint is not null)
        {
            var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TokenCacheTtl);
            _cache.Set(TokenKey(token),       endpoint, options);
            _cache.Set(IdKey(endpoint.Id), token,    options);
        }

        return endpoint;
    }

    /// <inheritdoc/>
    /// Evicts the cached token entry before the update so a stale entry is never served.
    /// The secondary id→token key allows us to find the OLD token even when the entity
    /// already carries a newly generated one (token regeneration flow).
    public override Task UpdateAsync(WebhookEndpoint entity, CancellationToken ct = default)
    {
        EvictById(entity.Id);
        return base.UpdateAsync(entity, ct);
    }

    /// <inheritdoc/>
    /// Evicts the cache entry for the endpoint being removed.
    public override Task DeleteAsync(WebhookEndpoint entity, CancellationToken ct = default)
    {
        EvictById(entity.Id);
        return base.DeleteAsync(entity, ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<WebhookEndpoint>> GetByWorkspaceIdAsync(Guid workspaceId, CancellationToken ct = default)
        => await _set
            .Where(e => e.WorkspaceId == workspaceId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<bool> TokenExistsAsync(string token, CancellationToken ct = default)
        => await _set.AnyAsync(e => e.Token == token, ct);

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// Looks up the currently cached token for an endpoint ID, then removes both keys.
    private void EvictById(Guid id)
    {
        if (_cache.TryGetValue(IdKey(id), out string? oldToken) && oldToken is not null)
            _cache.Remove(TokenKey(oldToken));

        _cache.Remove(IdKey(id));
    }
}
