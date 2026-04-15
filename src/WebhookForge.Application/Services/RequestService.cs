using WebhookForge.Application.Common.Helpers;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.Request;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Services;

/// <summary>
/// Captures incoming webhook requests and provides the query/purge API.
/// Header serialization is handled externally by the controller; this service works with strings.
/// Depends on: IUnitOfWork.
/// </summary>
public class RequestService : IRequestService
{
    private readonly IUnitOfWork _uow;

    public RequestService(IUnitOfWork uow) => _uow = uow;

    /// <inheritdoc/>
    public async Task<Result<PagedResult<IncomingRequestDto>>> GetByEndpointAsync(
        Guid endpointId, Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, endpointId, userId, ct);
        if (!access.IsSuccess) return Result<PagedResult<IncomingRequestDto>>.Forbidden(access.Error!);

        // Sequential awaits — EF Core DbContext is not thread-safe
        var items = await _uow.Requests.GetPagedByEndpointIdAsync(endpointId, page, pageSize, ct);
        var count = await _uow.Requests.GetCountByEndpointIdAsync(endpointId, ct);

        var paged = new PagedResult<IncomingRequestDto>
        {
            Items      = items.Select(ToDto).ToList(),
            TotalCount = count,
            Page       = page,
            PageSize   = pageSize
        };

        return Result<PagedResult<IncomingRequestDto>>.Success(paged);
    }

    /// <inheritdoc/>
    public async Task<Result<IncomingRequestDto>> GetByIdAsync(Guid requestId, Guid userId, CancellationToken ct = default)
    {
        var request = await _uow.Requests.GetByIdAsync(requestId, ct);
        if (request is null) return Result<IncomingRequestDto>.NotFound("Request not found.");

        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, request.EndpointId, userId, ct);
        if (!access.IsSuccess) return Result<IncomingRequestDto>.Forbidden(access.Error!);

        return Result<IncomingRequestDto>.Success(ToDto(request));
    }

    /// <inheritdoc/>
    public async Task<Result<IncomingRequestDto>> CaptureAsync(CaptureRequestDto dto, CancellationToken ct = default)
    {
        var endpoint = await _uow.Endpoints.GetByTokenAsync(dto.EndpointToken, ct);
        if (endpoint is null) return Result<IncomingRequestDto>.NotFound("Endpoint not found.");
        if (!endpoint.IsActive) return Result<IncomingRequestDto>.Failure("Endpoint is inactive.", 410);
        if (endpoint.ExpiresAt.HasValue && endpoint.ExpiresAt < DateTime.UtcNow)
            return Result<IncomingRequestDto>.Failure("Endpoint has expired.", 410);

        var request = new IncomingRequest
        {
            EndpointId  = endpoint.Id,
            Method      = dto.Method,
            Path        = dto.Path,
            Headers     = dto.Headers,
            QueryString = dto.QueryString,
            Body        = dto.Body,
            ContentType = dto.ContentType,
            IpAddress   = dto.IpAddress,
            SizeBytes   = dto.Body?.Length ?? 0,
            ReceivedAt  = DateTime.UtcNow
        };

        await _uow.Requests.AddAsync(request, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<IncomingRequestDto>.Success(ToDto(request));
    }

    /// <inheritdoc/>
    public async Task<Result<int>> PurgeAsync(Guid endpointId, int olderThanDays, Guid userId, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, endpointId, userId, ct);
        if (!access.IsSuccess) return Result<int>.Forbidden(access.Error!);

        var cutoff  = DateTime.UtcNow.AddDays(-olderThanDays);
        var deleted = await _uow.Requests.DeleteOlderThanAsync(endpointId, cutoff, ct);
        return Result<int>.Success(deleted);
    }

    // ── Mapping ──────────────────────────────────────────────────

    private static IncomingRequestDto ToDto(IncomingRequest r) => new()
    {
        Id          = r.Id,
        EndpointId  = r.EndpointId,
        Method      = r.Method,
        Headers     = JsonHelper.TryDeserialize<Dictionary<string, string>>(r.Headers),
        Path        = r.Path,
        QueryString = r.QueryString,
        Body        = r.Body,
        ContentType = r.ContentType,
        IpAddress   = r.IpAddress,
        SizeBytes   = r.SizeBytes,
        ReceivedAt  = r.ReceivedAt
    };
}
