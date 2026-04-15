using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.Request;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Business logic for capturing and querying incoming webhook requests.
/// Source: Application layer — implemented in Application/Services/RequestService.cs.
/// </summary>
public interface IRequestService
{
    /// <summary>Return a paginated list of captured requests for an endpoint.</summary>
    Task<Result<PagedResult<IncomingRequestDto>>> GetByEndpointAsync(
        Guid endpointId, Guid userId, int page, int pageSize, CancellationToken ct = default);

    /// <summary>Get the full detail of a single captured request.</summary>
    Task<Result<IncomingRequestDto>> GetByIdAsync(Guid requestId, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Store an incoming HTTP request and optionally match a mock rule.
    /// Called by <c>WebhookReceiverController</c> — no userId check (public endpoint).
    /// </summary>
    Task<Result<IncomingRequestDto>> CaptureAsync(CaptureRequestDto dto, CancellationToken ct = default);

    /// <summary>Delete all requests older than <paramref name="olderThanDays"/> days for an endpoint.</summary>
    Task<Result<int>> PurgeAsync(Guid endpointId, int olderThanDays, Guid userId, CancellationToken ct = default);
}
