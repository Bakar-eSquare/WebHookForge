using Microsoft.EntityFrameworkCore;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// Workspace-specific queries on top of the generic repository.
/// Source: Infrastructure layer — implements IWorkspaceRepository (Application).
/// </summary>
public class WorkspaceRepository : Repository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Workspace>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _set
            .Where(w => w.OwnerId == userId || w.Members.Any(m => m.UserId == userId))
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<Workspace?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _set.FirstOrDefaultAsync(w => w.Slug == slug, ct);

    /// <inheritdoc/>
    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
        => await _set.AnyAsync(w => w.Slug == slug, ct);

    /// <inheritdoc/>
    /// Eagerly loads Members and their User so access-guard checks and member lists work without extra queries.
    public async Task<Workspace?> GetWithMembersAsync(Guid workspaceId, CancellationToken ct = default)
        => await _set
            .Include(w => w.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(w => w.Id == workspaceId, ct);
}
