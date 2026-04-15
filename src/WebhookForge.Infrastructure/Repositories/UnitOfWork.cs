using Microsoft.EntityFrameworkCore.Storage;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// EF Core Unit of Work implementation.
/// Aggregates all repositories and wraps them in a single DbContext transaction boundary.
/// Source: Infrastructure layer — implements IUnitOfWork (Application).
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction?        _tx;

    public IUserRepository        Users         { get; }
    public IWorkspaceRepository   Workspaces    { get; }
    public IEndpointRepository    Endpoints     { get; }
    public IRequestRepository     Requests      { get; }
    public IMockRuleRepository    MockRules     { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }

    public UnitOfWork(
        ApplicationDbContext       context,
        IUserRepository            users,
        IWorkspaceRepository       workspaces,
        IEndpointRepository        endpoints,
        IRequestRepository         requests,
        IMockRuleRepository        mockRules,
        IRepository<RefreshToken>  refreshTokens)
    {
        _context      = context;
        Users         = users;
        Workspaces    = workspaces;
        Endpoints     = endpoints;
        Requests      = requests;
        MockRules     = mockRules;
        RefreshTokens = refreshTokens;
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _tx = await _context.Database.BeginTransactionAsync(ct);

    /// <inheritdoc/>
    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_tx is null) throw new InvalidOperationException("No active transaction to commit.");
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;
        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public void Dispose()
    {
        _tx?.Dispose();
        _context.Dispose();
    }
}
