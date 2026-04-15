using System.ComponentModel.DataAnnotations;

namespace WebhookForge.Application.DTOs.Workspace;

/// <summary>Request body for POST /api/workspaces.</summary>
public class CreateWorkspaceDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

/// <summary>Request body for PUT /api/workspaces/{id}.</summary>
public class UpdateWorkspaceDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

/// <summary>Request body for POST /api/workspaces/{id}/members.</summary>
public class AddMemberDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Role to assign: Viewer | Editor | Admin. Defaults to Viewer if unrecognised.</summary>
    [Required]
    public string Role { get; set; } = "Viewer";
}

/// <summary>Full workspace representation returned by list and detail endpoints.</summary>
public class WorkspaceDto
{
    public Guid    Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string  Slug        { get; set; } = string.Empty;
    public Guid    OwnerId     { get; set; }
    public DateTime CreatedAt  { get; set; }

    /// <summary>Members list — populated only by GetById, not by the list endpoint.</summary>
    public List<WorkspaceMemberDto> Members { get; set; } = new();
}

/// <summary>A single workspace member as returned inside <see cref="WorkspaceDto"/>.</summary>
public class WorkspaceMemberDto
{
    public Guid   UserId      { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Role        { get; set; } = string.Empty;
}
