using Microsoft.EntityFrameworkCore;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// MockRule-specific queries on top of the generic repository.
/// Source: Infrastructure layer — implements IMockRuleRepository (Application).
/// </summary>
public class MockRuleRepository : Repository<MockRule>, IMockRuleRepository
{
    public MockRuleRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc/>
    /// Returns only active rules ordered by Priority — used during live request matching.
    /// Backed by IX_MockRules_EndpointId_Priority index.
    public async Task<IReadOnlyList<MockRule>> GetActiveByEndpointIdAsync(Guid endpointId, CancellationToken ct = default)
        => await _set
            .Where(r => r.EndpointId == endpointId && r.IsActive)
            .OrderBy(r => r.Priority)
            .ToListAsync(ct);
}
