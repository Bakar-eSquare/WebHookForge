using WebhookForge.Domain.Common;

namespace WebhookForge.Domain.Entities;

/// <summary>
/// Represents a single HTTP request captured by a webhook endpoint.
/// Headers are persisted as a JSON string to avoid a separate key-value table.
/// Purged automatically based on workspace retention settings.
/// </summary>
public class IncomingRequest : BaseEntity
{
    /// <summary>FK → Endpoints.Id.</summary>
    public Guid EndpointId { get; set; }

    /// <summary>HTTP verb (GET, POST, PUT, etc.) — max 10 chars.</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>Request path portion of the URL (excluding the token prefix).</summary>
    public string? Path { get; set; }

    /// <summary>Raw query string (e.g. "?foo=bar&amp;baz=1").</summary>
    public string? QueryString { get; set; }

    /// <summary>All request headers serialized as JSON Dictionary&lt;string,string&gt;.</summary>
    public string? Headers { get; set; }

    /// <summary>Raw request body — stored as-is regardless of content type.</summary>
    public string? Body { get; set; }

    /// <summary>Content-Type header value (max 200 chars).</summary>
    public string? ContentType { get; set; }

    /// <summary>Caller's IP address (IPv4 or IPv6, max 45 chars).</summary>
    public string? IpAddress { get; set; }

    /// <summary>Body size in bytes — 0 for requests with no body.</summary>
    public int SizeBytes { get; set; }

    /// <summary>UTC time the request was received at the API.</summary>
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    // ── Navigation ──────────────────────────────────────────────
    public WebhookEndpoint Endpoint { get; set; } = null!;
}
