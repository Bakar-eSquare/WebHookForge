using WebhookForge.Application.Common.Helpers;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.Endpoint;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Services;

/// <summary>
/// Handles endpoint CRUD and token regeneration.
/// Workspace membership is verified via <see cref="AccessGuard"/> before every mutating operation.
/// Depends on: IUnitOfWork.
/// </summary>
public class EndpointService : IEndpointService
{
    private readonly IUnitOfWork _uow;

    public EndpointService(IUnitOfWork uow) => _uow = uow;

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<EndpointDto>>> GetByWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireWorkspaceMemberAsync(_uow, workspaceId, userId, ct);
        if (!access.IsSuccess) return Result<IReadOnlyList<EndpointDto>>.Forbidden(access.Error!);

        // Load endpoints and their request counts in parallel
        var endpointsTask   = _uow.Endpoints.GetByWorkspaceIdAsync(workspaceId, ct);
        // Request counts are fetched per-endpoint below — batch query avoids N+1
        var endpoints = await endpointsTask;

        // Sequential per-endpoint count — EF Core DbContext is not thread-safe
        var counts = new int[endpoints.Count];
        for (var i = 0; i < endpoints.Count; i++)
            counts[i] = await _uow.Requests.GetCountByEndpointIdAsync(endpoints[i].Id, ct);

        var result = endpoints.Select((e, i) => ToDto(e, counts[i])).ToList();
        return Result<IReadOnlyList<EndpointDto>>.Success(result);
    }

    /// <inheritdoc/>
    public async Task<Result<EndpointDto>> GetByIdAsync(Guid endpointId, Guid userId, CancellationToken ct = default)
    {
        var endpoint = await _uow.Endpoints.GetByIdAsync(endpointId, ct);
        if (endpoint is null) return Result<EndpointDto>.NotFound("Endpoint not found.");

        var access = await AccessGuard.RequireWorkspaceMemberAsync(_uow, endpoint.WorkspaceId, userId, ct);
        if (!access.IsSuccess) return Result<EndpointDto>.Forbidden(access.Error!);

        var count = await _uow.Requests.GetCountByEndpointIdAsync(endpointId, ct);
        return Result<EndpointDto>.Success(ToDto(endpoint, count));
    }

    /// <inheritdoc/>
    public async Task<Result<EndpointDto>> CreateAsync(CreateEndpointDto dto, Guid workspaceId, Guid userId, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireWorkspaceMemberAsync(_uow, workspaceId, userId, ct);
        if (!access.IsSuccess) return Result<EndpointDto>.Forbidden(access.Error!);

        var token = await GenerateUniqueTokenAsync(ct);
        var endpoint = new WebhookEndpoint
        {
            WorkspaceId = workspaceId,
            Token       = token,
            Name        = dto.Name,
            Description = dto.Description,
            ExpiresAt   = dto.ExpiresAt
        };

        await _uow.Endpoints.AddAsync(endpoint, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<EndpointDto>.Success(ToDto(endpoint, 0), 201);
    }

    /// <inheritdoc/>
    public async Task<Result<EndpointDto>> UpdateAsync(Guid endpointId, UpdateEndpointDto dto, Guid userId, CancellationToken ct = default)
    {
        var endpoint = await _uow.Endpoints.GetByIdAsync(endpointId, ct);
        if (endpoint is null) return Result<EndpointDto>.NotFound("Endpoint not found.");

        var access = await AccessGuard.RequireWorkspaceMemberAsync(_uow, endpoint.WorkspaceId, userId, ct);
        if (!access.IsSuccess) return Result<EndpointDto>.Forbidden(access.Error!);

        endpoint.Name        = dto.Name;
        endpoint.Description = dto.Description;
        endpoint.IsActive    = dto.IsActive;
        endpoint.ExpiresAt   = dto.ExpiresAt;
        endpoint.UpdatedAt   = DateTime.UtcNow;

        await _uow.Endpoints.UpdateAsync(endpoint, ct);
        await _uow.SaveChangesAsync(ct);

        var count = await _uow.Requests.GetCountByEndpointIdAsync(endpointId, ct);
        return Result<EndpointDto>.Success(ToDto(endpoint, count));
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid endpointId, Guid userId, CancellationToken ct = default)
    {
        var endpoint = await _uow.Endpoints.GetByIdAsync(endpointId, ct);
        if (endpoint is null) return Result.NotFound("Endpoint not found.");

        var access = await AccessGuard.RequireWorkspaceMemberAsync(_uow, endpoint.WorkspaceId, userId, ct);
        if (!access.IsSuccess) return Result.Forbidden(access.Error!);

        await _uow.Endpoints.DeleteAsync(endpoint, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result<EndpointDto>> RegenerateTokenAsync(Guid endpointId, Guid userId, CancellationToken ct = default)
    {
        var endpoint = await _uow.Endpoints.GetByIdAsync(endpointId, ct);
        if (endpoint is null) return Result<EndpointDto>.NotFound("Endpoint not found.");

        var access = await AccessGuard.RequireWorkspaceMemberAsync(_uow, endpoint.WorkspaceId, userId, ct);
        if (!access.IsSuccess) return Result<EndpointDto>.Forbidden(access.Error!);

        endpoint.Token     = await GenerateUniqueTokenAsync(ct);
        endpoint.UpdatedAt = DateTime.UtcNow;

        await _uow.Endpoints.UpdateAsync(endpoint, ct);
        await _uow.SaveChangesAsync(ct);

        var count = await _uow.Requests.GetCountByEndpointIdAsync(endpointId, ct);
        return Result<EndpointDto>.Success(ToDto(endpoint, count));
    }

    // ── Helpers ──────────────────────────────────────────────────

    private async Task<string> GenerateUniqueTokenAsync(CancellationToken ct)
    {
        string token;
        do { token = Guid.NewGuid().ToString("N")[..24]; }
        while (await _uow.Endpoints.TokenExistsAsync(token, ct));
        return token;
    }

    private static EndpointDto ToDto(WebhookEndpoint ep, int requestCount) => new()
    {
        Id           = ep.Id,
        WorkspaceId  = ep.WorkspaceId,
        Token        = ep.Token,
        Name         = ep.Name,
        Description  = ep.Description,
        IsActive     = ep.IsActive,
        CreatedAt    = ep.CreatedAt,
        ExpiresAt    = ep.ExpiresAt,
        WebhookUrl   = $"/hooks/{ep.Token}",
        RequestCount = requestCount
    };
}
