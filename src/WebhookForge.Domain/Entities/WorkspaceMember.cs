using WebhookForge.Domain.Common;
using WebhookForge.Domain.Enums;

namespace WebhookForge.Domain.Entities;

/// <summary>
/// Join entity between Workspace and User.
/// Stores the role the user holds within that workspace.
/// The unique constraint UX_WorkspaceMembers_WorkspaceUser prevents duplicate memberships.
/// </summary>
public class WorkspaceMember : BaseEntity
{
    /// <summary>FK → Workspaces.Id.</summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>FK → Users.Id.</summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Role within this workspace: Viewer (read-only), Editor (manage endpoints/rules),
    /// Admin (manage members + full access).
    /// </summary>
    public WorkspaceRole Role { get; set; } = WorkspaceRole.Viewer;

    // ── Navigation ──────────────────────────────────────────────
    public Workspace Workspace { get; set; } = null!;
    public User      User      { get; set; } = null!;
}
