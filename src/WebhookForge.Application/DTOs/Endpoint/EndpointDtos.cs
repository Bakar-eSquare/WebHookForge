using System.ComponentModel.DataAnnotations;

namespace WebhookForge.Application.DTOs.Endpoint;

/// <summary>Request body for POST /api/workspaces/{id}/endpoints.</summary>
public class CreateEndpointDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>Optional hard expiry for this endpoint. Null means it never expires.</summary>
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>Request body for PUT /api/endpoints/{id}.</summary>
public class UpdateEndpointDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool     IsActive  { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>Endpoint representation returned by list and detail endpoints.</summary>
public class EndpointDto
{
    public Guid      Id           { get; set; }
    public Guid      WorkspaceId  { get; set; }

    /// <summary>The unique token — append to /hooks/ to get the public webhook URL.</summary>
    public string    Token        { get; set; } = string.Empty;
    public string    Name         { get; set; } = string.Empty;
    public string?   Description  { get; set; }
    public bool      IsActive     { get; set; }
    public DateTime  CreatedAt    { get; set; }
    public DateTime? ExpiresAt    { get; set; }

    /// <summary>Fully-qualified webhook URL (relative to API base).</summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>Total captured requests for this endpoint.</summary>
    public int RequestCount { get; set; }
}
