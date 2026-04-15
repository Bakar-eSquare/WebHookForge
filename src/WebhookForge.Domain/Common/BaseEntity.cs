namespace WebhookForge.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides a sequential GUID primary key (matches DB NEWSEQUENTIALID()),
/// a creation timestamp, and an optional last-updated timestamp.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key — auto-generated sequential GUID.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>UTC timestamp set on INSERT by the database default (GETUTCDATE()).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp updated by EF Core's SaveChangesAsync override on every UPDATE.</summary>
    public DateTime? UpdatedAt { get; set; }
}
