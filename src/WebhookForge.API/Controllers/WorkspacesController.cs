using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.DTOs.Workspace;

namespace WebhookForge.API.Controllers;

/// <summary>
/// Workspace CRUD and member management.
/// All endpoints require authentication. Access control (owner vs member) is enforced in the service layer.
/// </summary>
[Authorize]
public class WorkspacesController : BaseController
{
    private readonly IWorkspaceService _workspaces;
    public WorkspacesController(IWorkspaceService workspaces) => _workspaces = workspaces;

    /// <summary>List all workspaces the current user belongs to.</summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => ToActionResult(await _workspaces.GetUserWorkspacesAsync(CurrentUserId, ct));

    /// <summary>Get a workspace by ID. Returns 403 if the user is not a member.</summary>
    [HttpGet("{id:guid}", Name = "GetWorkspace")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => ToActionResult(await _workspaces.GetByIdAsync(id, CurrentUserId, ct));

    /// <summary>Create a new workspace. The creator is automatically made Admin.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto dto, CancellationToken ct)
    {
        var result = await _workspaces.CreateAsync(dto, CurrentUserId, ct);
        return ToCreatedResult(result, "GetWorkspace", new { id = result.Value?.Id });
    }

    /// <summary>Update workspace name. Only the owner may update.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkspaceDto dto, CancellationToken ct)
        => ToActionResult(await _workspaces.UpdateAsync(id, dto, CurrentUserId, ct));

    /// <summary>Delete the workspace. Only the owner may delete.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => ToActionResult(await _workspaces.DeleteAsync(id, CurrentUserId, ct));

    /// <summary>Invite a user (by email) to the workspace. Requires Admin role.</summary>
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberDto dto, CancellationToken ct)
        => ToActionResult(await _workspaces.AddMemberAsync(id, dto, CurrentUserId, ct));

    /// <summary>Remove a member from the workspace. Only the owner may remove.</summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken ct)
        => ToActionResult(await _workspaces.RemoveMemberAsync(id, userId, CurrentUserId, ct));
}
