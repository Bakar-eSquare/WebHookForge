using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.Common.Models;

namespace WebhookForge.Application.Common.Helpers;

/// <summary>
/// Centralises the workspace/endpoint membership check that was previously
/// duplicated across EndpointService, RequestService, and MockRuleService.
///
/// Usage in a service method:
///   var check = await AccessGuard.RequireWorkspaceMemberAsync(_uow, workspaceId, userId, ct);
///   if (!check.IsSuccess) return Result&lt;T&gt;.Forbidden(check.Error!);
/// </summary>
public static class AccessGuard
{
    /// <summary>
    /// Verifies that <paramref name="userId"/> is the workspace owner or a member.
    /// Returns Success when access is granted, Forbidden/NotFound otherwise.
    /// </summary>
    public static async Task<Result> RequireWorkspaceMemberAsync(
        IUnitOfWork uow,
        Guid        workspaceId,
        Guid        userId,
        CancellationToken ct = default)
    {
        var workspace = await uow.Workspaces.GetWithMembersAsync(workspaceId, ct);
        if (workspace is null)
            return Result.NotFound("Workspace not found.");

        var isMember = workspace.OwnerId == userId
                    || workspace.Members.Any(m => m.UserId == userId);

        return isMember
            ? Result.Success()
            : Result.Forbidden("You do not have access to this workspace.");
    }

    /// <summary>
    /// Resolves the endpoint's workspace and verifies membership.
    /// Returns Success when access is granted, NotFound/Forbidden otherwise.
    /// </summary>
    public static async Task<Result> RequireEndpointAccessAsync(
        IUnitOfWork uow,
        Guid        endpointId,
        Guid        userId,
        CancellationToken ct = default)
    {
        var endpoint = await uow.Endpoints.GetByIdAsync(endpointId, ct);
        if (endpoint is null)
            return Result.NotFound("Endpoint not found.");

        return await RequireWorkspaceMemberAsync(uow, endpoint.WorkspaceId, userId, ct);
    }
}
