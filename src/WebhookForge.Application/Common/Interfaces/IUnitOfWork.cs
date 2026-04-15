using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Unit of Work — aggregates all domain repositories into a single transaction boundary.
/// Services inject <see cref="IUnitOfWork"/> and call <see cref="SaveChangesAsync"/>
/// once after staging all changes, ensuring atomicity.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Repository for <c>User</c> entities.</summary>
    IUserRepository Users { get; }

    /// <summary>Repository for <c>Workspace</c> entities.</summary>
    IWorkspaceRepository Workspaces { get; }

    /// <summary>Repository for <c>WebhookEndpoint</c> entities.</summary>
    IEndpointRepository Endpoints { get; }

    /// <summary>Repository for <c>IncomingRequest</c> entities.</summary>
    IRequestRepository Requests { get; }

    /// <summary>Repository for <c>MockRule</c> entities.</summary>
    IMockRuleRepository MockRules { get; }

    /// <summary>
    /// Generic repository for <c>RefreshToken</c> — used by AuthService for token rotation.
    /// No domain-specific queries needed beyond the generic IRepository contract.
    /// </summary>
    IRepository<RefreshToken> RefreshTokens { get; }

    /// <summary>Persist all staged changes in a single database round-trip. Returns rows affected.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>Begin an explicit database transaction.</summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>Commit the current explicit transaction.</summary>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>Roll back the current explicit transaction. Safe to call when no transaction is active.</summary>
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
