using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.MockRule;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Business logic for mock rule CRUD, activation toggle, reordering, and request matching.
/// Source: Application layer — implemented in Application/Services/MockRuleService.cs.
/// </summary>
public interface IMockRuleService
{
    /// <summary>List all rules for an endpoint ordered by Priority. User must be a workspace member.</summary>
    Task<Result<IReadOnlyList<MockRuleDto>>> GetByEndpointAsync(Guid endpointId, Guid userId, CancellationToken ct = default);

    /// <summary>Get a single rule by ID.</summary>
    Task<Result<MockRuleDto>> GetByIdAsync(Guid ruleId, Guid userId, CancellationToken ct = default);

    /// <summary>Create a new mock rule for an endpoint.</summary>
    Task<Result<MockRuleDto>> CreateAsync(CreateMockRuleDto dto, Guid endpointId, Guid userId, CancellationToken ct = default);

    /// <summary>Update all editable fields of a rule.</summary>
    Task<Result<MockRuleDto>> UpdateAsync(Guid ruleId, UpdateMockRuleDto dto, Guid userId, CancellationToken ct = default);

    /// <summary>Delete a mock rule.</summary>
    Task<Result> DeleteAsync(Guid ruleId, Guid userId, CancellationToken ct = default);

    /// <summary>Toggle IsActive on a rule without touching other fields.</summary>
    Task<Result<MockRuleDto>> ToggleAsync(Guid ruleId, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Reassign Priority values based on the supplied ordered list of rule IDs.
    /// First ID gets priority 10, second gets 20, and so on (gap of 10 allows future insertion).
    /// </summary>
    Task<Result> ReorderAsync(Guid endpointId, ReorderRulesDto dto, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Evaluate active rules against an incoming request and return the first match.
    /// Returns null when no rule matches (caller should fall back to default 200 OK).
    /// </summary>
    Task<MockRuleDto?> MatchRuleAsync(Guid endpointId, string method, string? path, string? body, CancellationToken ct = default);
}
