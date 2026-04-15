using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Extends the generic repository with webhook endpoint-specific queries.
/// Source: Application layer — implemented in Infrastructure/Repositories/EndpointRepository.cs.
/// </summary>
public interface IEndpointRepository : IRepository<WebhookEndpoint>
{
    /// <summary>
    /// Look up an endpoint by its public token.
    /// Called on every incoming webhook request — backed by a unique index (UX_Endpoints_Token).
    /// </summary>
    Task<WebhookEndpoint?> GetByTokenAsync(string token, CancellationToken ct = default);

    /// <summary>Returns all endpoints belonging to the given workspace, newest first.</summary>
    Task<IReadOnlyList<WebhookEndpoint>> GetByWorkspaceIdAsync(Guid workspaceId, CancellationToken ct = default);

    /// <summary>Returns true if any endpoint already uses this token value.</summary>
    Task<bool> TokenExistsAsync(string token, CancellationToken ct = default);
}
