using WebhookForge.Domain.Common;

namespace WebhookForge.Domain.Entities;

/// <summary>
/// A unique webhook endpoint inside a workspace.
/// Incoming HTTP requests are received at /hooks/{Token} and stored as <see cref="IncomingRequest"/> records.
/// Mock rules attached to an endpoint control what response is returned to the caller.
/// </summary>
public class WebhookEndpoint : BaseEntity
{
    /// <summary>FK → Workspaces.Id.</summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// URL-safe token used in the public webhook URL (/hooks/{Token}).
    /// Auto-generated on creation; can be regenerated via the API.
    /// Unique across the entire platform (UX_Endpoints_Token).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Human-readable name for the endpoint (max 100 chars).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional description shown in the dashboard (max 500 chars).</summary>
    public string? Description { get; set; }

    /// <summary>When false, incoming requests are rejected with 404.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Optional UTC expiry — requests after this time are rejected.</summary>
    public DateTime? ExpiresAt { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    /// <summary>The workspace this endpoint belongs to.</summary>
    public Workspace Workspace { get; set; } = null!;

    /// <summary>All HTTP requests captured by this endpoint.</summary>
    public ICollection<IncomingRequest> Requests { get; set; } = new List<IncomingRequest>();

    /// <summary>Mock rules evaluated on each incoming request in priority order.</summary>
    public ICollection<MockRule> MockRules { get; set; } = new List<MockRule>();
}
