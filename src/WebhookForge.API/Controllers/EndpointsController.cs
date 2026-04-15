using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.DTOs.Endpoint;

namespace WebhookForge.API.Controllers;

/// <summary>
/// Webhook endpoint CRUD and token management.
/// All endpoints require authentication.
/// </summary>
[Authorize]
[ApiController]
[Route("api")]
public class EndpointsController : BaseController
{
    private readonly IEndpointService _endpoints;
    public EndpointsController(IEndpointService endpoints) => _endpoints = endpoints;

    /// <summary>List all endpoints in a workspace. User must be a member.</summary>
    [HttpGet("workspaces/{workspaceId:guid}/endpoints")]
    public async Task<IActionResult> List(Guid workspaceId, CancellationToken ct)
        => ToActionResult(await _endpoints.GetByWorkspaceAsync(workspaceId, CurrentUserId, ct));

    /// <summary>Get a single endpoint by ID.</summary>
    [HttpGet("endpoints/{id:guid}", Name = "GetEndpoint")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => ToActionResult(await _endpoints.GetByIdAsync(id, CurrentUserId, ct));

    /// <summary>Create a new endpoint inside a workspace.</summary>
    [HttpPost("workspaces/{workspaceId:guid}/endpoints")]
    public async Task<IActionResult> Create(Guid workspaceId, [FromBody] CreateEndpointDto dto, CancellationToken ct)
    {
        var result = await _endpoints.CreateAsync(dto, workspaceId, CurrentUserId, ct);
        return ToCreatedResult(result, "GetEndpoint", new { id = result.Value?.Id });
    }

    /// <summary>Update an endpoint's name, description, or active state.</summary>
    [HttpPut("endpoints/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEndpointDto dto, CancellationToken ct)
        => ToActionResult(await _endpoints.UpdateAsync(id, dto, CurrentUserId, ct));

    /// <summary>Delete an endpoint and all its captured requests.</summary>
    [HttpDelete("endpoints/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => ToActionResult(await _endpoints.DeleteAsync(id, CurrentUserId, ct));

    /// <summary>Regenerate the endpoint token, invalidating the old webhook URL.</summary>
    [HttpPost("endpoints/{id:guid}/regenerate-token")]
    public async Task<IActionResult> RegenerateToken(Guid id, CancellationToken ct)
        => ToActionResult(await _endpoints.RegenerateTokenAsync(id, CurrentUserId, ct));
}
