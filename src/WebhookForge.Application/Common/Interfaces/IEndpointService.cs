using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.Endpoint;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Business logic for webhook endpoint CRUD and token management.
/// Source: Application layer — implemented in Application/Services/EndpointService.cs.
/// </summary>
public interface IEndpointService
{
    /// <summary>List all endpoints in a workspace. User must be a workspace member.</summary>
    Task<Result<IReadOnlyList<EndpointDto>>> GetByWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken ct = default);

    /// <summary>Get a single endpoint. User must be a member of the owning workspace.</summary>
    Task<Result<EndpointDto>> GetByIdAsync(Guid endpointId, Guid userId, CancellationToken ct = default);

    /// <summary>Create a new endpoint with an auto-generated unique token.</summary>
    Task<Result<EndpointDto>> CreateAsync(CreateEndpointDto dto, Guid workspaceId, Guid userId, CancellationToken ct = default);

    /// <summary>Update endpoint name/description/active state.</summary>
    Task<Result<EndpointDto>> UpdateAsync(Guid endpointId, UpdateEndpointDto dto, Guid userId, CancellationToken ct = default);

    /// <summary>Delete an endpoint and all its captured requests.</summary>
    Task<Result> DeleteAsync(Guid endpointId, Guid userId, CancellationToken ct = default);

    /// <summary>Generate a new random token for the endpoint, invalidating the old webhook URL.</summary>
    Task<Result<EndpointDto>> RegenerateTokenAsync(Guid endpointId, Guid userId, CancellationToken ct = default);
}
