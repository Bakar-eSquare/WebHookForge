using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Extends the generic repository with workspace-specific queries.
/// Source: Application layer — implemented in Infrastructure/Repositories/WorkspaceRepository.cs.
/// </summary>
public interface IWorkspaceRepository : IRepository<Workspace>
{
    /// <summary>Returns all workspaces where the given user is owner or member.</summary>
    Task<IReadOnlyList<Workspace>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Find a workspace by its URL slug. Returns null if not found.</summary>
    Task<Workspace?> GetBySlugAsync(string slug, CancellationToken ct = default);

    /// <summary>Returns true if a workspace with the given slug already exists.</summary>
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);

    /// <summary>
    /// Returns the workspace with its Members and their User navigation properties loaded.
    /// Used by access-guard checks and member management operations.
    /// </summary>
    Task<Workspace?> GetWithMembersAsync(Guid workspaceId, CancellationToken ct = default);
}
