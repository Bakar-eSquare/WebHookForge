using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.Workspace;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Business logic for workspace CRUD and member management.
/// Source: Application layer — implemented in Application/Services/WorkspaceService.cs.
/// </summary>
public interface IWorkspaceService
{
    /// <summary>List all workspaces the user owns or belongs to.</summary>
    Task<Result<IReadOnlyList<WorkspaceDto>>> GetUserWorkspacesAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Get a single workspace. Fails if not found or user is not a member.</summary>
    Task<Result<WorkspaceDto>> GetByIdAsync(Guid workspaceId, Guid userId, CancellationToken ct = default);

    /// <summary>Create a workspace; the creator is automatically added as Admin.</summary>
    Task<Result<WorkspaceDto>> CreateAsync(CreateWorkspaceDto dto, Guid userId, CancellationToken ct = default);

    /// <summary>Update workspace name. Only the owner may update.</summary>
    Task<Result<WorkspaceDto>> UpdateAsync(Guid workspaceId, UpdateWorkspaceDto dto, Guid userId, CancellationToken ct = default);

    /// <summary>Delete a workspace and all its endpoints/requests. Only the owner may delete.</summary>
    Task<Result> DeleteAsync(Guid workspaceId, Guid userId, CancellationToken ct = default);

    /// <summary>Add a user (by email) to the workspace. Requires Admin role.</summary>
    Task<Result> AddMemberAsync(Guid workspaceId, AddMemberDto dto, Guid requestingUserId, CancellationToken ct = default);

    /// <summary>Remove a member from the workspace. Only the owner may remove.</summary>
    Task<Result> RemoveMemberAsync(Guid workspaceId, Guid memberUserId, Guid requestingUserId, CancellationToken ct = default);
}
