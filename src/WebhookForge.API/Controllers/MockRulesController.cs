using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.DTOs.MockRule;

namespace WebhookForge.API.Controllers;

/// <summary>
/// Mock rule CRUD, toggle, and reorder.
/// All endpoints require authentication.
/// </summary>
[Authorize]
[ApiController]
[Route("api")]
public class MockRulesController : BaseController
{
    private readonly IMockRuleService _rules;
    public MockRulesController(IMockRuleService rules) => _rules = rules;

    /// <summary>List all mock rules for an endpoint, ordered by Priority.</summary>
    [HttpGet("endpoints/{endpointId:guid}/rules")]
    public async Task<IActionResult> List(Guid endpointId, CancellationToken ct)
        => ToActionResult(await _rules.GetByEndpointAsync(endpointId, CurrentUserId, ct));

    /// <summary>Get a single rule by ID.</summary>
    [HttpGet("rules/{id:guid}", Name = "GetMockRule")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => ToActionResult(await _rules.GetByIdAsync(id, CurrentUserId, ct));

    /// <summary>Create a new mock rule for an endpoint.</summary>
    [HttpPost("endpoints/{endpointId:guid}/rules")]
    public async Task<IActionResult> Create(Guid endpointId, [FromBody] CreateMockRuleDto dto, CancellationToken ct)
    {
        var result = await _rules.CreateAsync(dto, endpointId, CurrentUserId, ct);
        return ToCreatedResult(result, "GetMockRule", new { id = result.Value?.Id });
    }

    /// <summary>Update all editable fields of a rule.</summary>
    [HttpPut("rules/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMockRuleDto dto, CancellationToken ct)
        => ToActionResult(await _rules.UpdateAsync(id, dto, CurrentUserId, ct));

    /// <summary>Delete a mock rule.</summary>
    [HttpDelete("rules/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => ToActionResult(await _rules.DeleteAsync(id, CurrentUserId, ct));

    /// <summary>Toggle IsActive without changing other rule fields.</summary>
    [HttpPatch("rules/{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id, CancellationToken ct)
        => ToActionResult(await _rules.ToggleAsync(id, CurrentUserId, ct));

    /// <summary>Reassign rule priorities based on supplied ordered list of IDs.</summary>
    [HttpPut("endpoints/{endpointId:guid}/rules/reorder")]
    public async Task<IActionResult> Reorder(Guid endpointId, [FromBody] ReorderRulesDto dto, CancellationToken ct)
        => ToActionResult(await _rules.ReorderAsync(endpointId, dto, CurrentUserId, ct));
}
