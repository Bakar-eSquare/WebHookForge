using WebhookForge.Application.Common.Helpers;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.Workspace;
using WebhookForge.Domain.Entities;
using WebhookForge.Domain.Enums;

namespace WebhookForge.Application.Services;

/// <summary>
/// Handles workspace CRUD and member management.
/// Access checks are delegated to <see cref="AccessGuard"/>.
/// Depends on: IUnitOfWork.
/// </summary>
public class WorkspaceService : IWorkspaceService
{
    private readonly IUnitOfWork _uow;

    public WorkspaceService(IUnitOfWork uow) => _uow = uow;

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<WorkspaceDto>>> GetUserWorkspacesAsync(Guid userId, CancellationToken ct = default)
    {
        var workspaces = await _uow.Workspaces.GetByUserIdAsync(userId, ct);
        return Result<IReadOnlyList<WorkspaceDto>>.Success(workspaces.Select(ToDto).ToList());
    }

    /// <inheritdoc/>
    public async Task<Result<WorkspaceDto>> GetByIdAsync(Guid workspaceId, Guid userId, CancellationToken ct = default)
    {
        var workspace = await _uow.Workspaces.GetWithMembersAsync(workspaceId, ct);
        if (workspace is null)
            return Result<WorkspaceDto>.NotFound("Workspace not found.");

        var isMember = workspace.OwnerId == userId
                    || workspace.Members.Any(m => m.UserId == userId);

        return isMember
            ? Result<WorkspaceDto>.Success(ToDto(workspace))
            : Result<WorkspaceDto>.Forbidden();
    }

    /// <inheritdoc/>
    public async Task<Result<WorkspaceDto>> CreateAsync(CreateWorkspaceDto dto, Guid userId, CancellationToken ct = default)
    {
        var slug = GenerateSlug(dto.Name);
        if (await _uow.Workspaces.SlugExistsAsync(slug, ct))
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..6]}";

        var workspace = new Workspace
        {
            Name    = dto.Name,
            Slug    = slug,
            OwnerId = userId,
            Members = { new WorkspaceMember { UserId = userId, Role = WorkspaceRole.Admin } }
        };

        await _uow.Workspaces.AddAsync(workspace, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<WorkspaceDto>.Success(ToDto(workspace), 201);
    }

    /// <inheritdoc/>
    public async Task<Result<WorkspaceDto>> UpdateAsync(Guid workspaceId, UpdateWorkspaceDto dto, Guid userId, CancellationToken ct = default)
    {
        var workspace = await _uow.Workspaces.GetByIdAsync(workspaceId, ct);
        if (workspace is null) return Result<WorkspaceDto>.NotFound("Workspace not found.");
        if (workspace.OwnerId != userId) return Result<WorkspaceDto>.Forbidden("Only the owner can update this workspace.");

        workspace.Name      = dto.Name;
        workspace.UpdatedAt = DateTime.UtcNow;

        await _uow.Workspaces.UpdateAsync(workspace, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<WorkspaceDto>.Success(ToDto(workspace));
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid workspaceId, Guid userId, CancellationToken ct = default)
    {
        var workspace = await _uow.Workspaces.GetByIdAsync(workspaceId, ct);
        if (workspace is null) return Result.NotFound("Workspace not found.");
        if (workspace.OwnerId != userId) return Result.Forbidden("Only the owner can delete this workspace.");

        await _uow.Workspaces.DeleteAsync(workspace, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result> AddMemberAsync(Guid workspaceId, AddMemberDto dto, Guid requestingUserId, CancellationToken ct = default)
    {
        // Load workspace with members and look up invited user in parallel
        var wsTask   = _uow.Workspaces.GetWithMembersAsync(workspaceId, ct);
        var userTask = _uow.Users.GetByEmailAsync(dto.Email, ct);
        await Task.WhenAll(wsTask, userTask);

        var workspace    = wsTask.Result;
        var invitedUser  = userTask.Result;

        if (workspace is null)   return Result.NotFound("Workspace not found.");
        if (invitedUser is null) return Result.NotFound($"No user found with email '{dto.Email}'.");

        var requester = workspace.Members.FirstOrDefault(m => m.UserId == requestingUserId);
        if (requester?.Role != WorkspaceRole.Admin && workspace.OwnerId != requestingUserId)
            return Result.Forbidden("Only admins can add members.");

        if (workspace.Members.Any(m => m.UserId == invitedUser.Id))
            return Result.Failure("User is already a member of this workspace.");

        var role = Enum.TryParse<WorkspaceRole>(dto.Role, ignoreCase: true, out var parsed)
                 ? parsed
                 : WorkspaceRole.Viewer;

        workspace.Members.Add(new WorkspaceMember { UserId = invitedUser.Id, Role = role });
        await _uow.Workspaces.UpdateAsync(workspace, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result> RemoveMemberAsync(Guid workspaceId, Guid memberUserId, Guid requestingUserId, CancellationToken ct = default)
    {
        var workspace = await _uow.Workspaces.GetWithMembersAsync(workspaceId, ct);
        if (workspace is null) return Result.NotFound("Workspace not found.");
        if (workspace.OwnerId != requestingUserId) return Result.Forbidden("Only the owner can remove members.");

        var member = workspace.Members.FirstOrDefault(m => m.UserId == memberUserId);
        if (member is null) return Result.NotFound("Member not found.");

        workspace.Members.Remove(member);
        await _uow.Workspaces.UpdateAsync(workspace, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    // ── Mapping ──────────────────────────────────────────────────
    private static WorkspaceDto ToDto(Workspace w) => new()
    {
        Id        = w.Id,
        Name      = w.Name,
        Slug      = w.Slug,
        OwnerId   = w.OwnerId,
        CreatedAt = w.CreatedAt,
        Members   = w.Members.Select(m => new WorkspaceMemberDto
        {
            UserId      = m.UserId,
            DisplayName = m.User?.DisplayName ?? string.Empty,
            Email       = m.User?.Email       ?? string.Empty,
            Role        = m.Role.ToString()
        }).ToList()
    };

    /// <summary>Lowercase, trim, replace spaces/underscores with hyphens.</summary>
    private static string GenerateSlug(string name)
        => name.ToLowerInvariant().Trim()
               .Replace(' ', '-')
               .Replace('_', '-');
}
