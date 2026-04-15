using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Extends the generic repository with mock-rule-specific queries.
/// Source: Application layer — implemented in Infrastructure/Repositories/MockRuleRepository.cs.
/// </summary>
public interface IMockRuleRepository : IRepository<MockRule>
{
    /// <summary>
    /// Returns all active rules for an endpoint ordered by Priority ascending.
    /// Called on every incoming webhook request to find a matching mock response.
    /// </summary>
    Task<IReadOnlyList<MockRule>> GetActiveByEndpointIdAsync(Guid endpointId, CancellationToken ct = default);
}
