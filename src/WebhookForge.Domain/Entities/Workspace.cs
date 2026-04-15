using WebhookForge.Domain.Common;

namespace WebhookForge.Domain.Entities;

/// <summary>
/// A workspace groups related webhook endpoints together.
/// Every workspace has an owner (User) and zero or more members.
/// </summary>
public class Workspace : BaseEntity
{
    /// <summary>FK → Users.Id. The user who created and owns this workspace.</summary>
    public Guid OwnerId { get; set; }

    /// <summary>Display name of the workspace (max 100 chars).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-safe slug derived from Name, used in webhook URLs.
    /// Guaranteed unique; a short random suffix is appended on collision.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    // ── Navigation ──────────────────────────────────────────────
    /// <summary>The user who owns this workspace.</summary>
    public User Owner { get; set; } = null!;

    /// <summary>All members (including the owner) of this workspace.</summary>
    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();

    /// <summary>Webhook endpoints that live inside this workspace.</summary>
    public ICollection<WebhookEndpoint> Endpoints { get; set; } = new List<WebhookEndpoint>();
}
