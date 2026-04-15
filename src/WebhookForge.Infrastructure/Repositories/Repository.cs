using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Common;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// Generic EF Core repository implementation.
/// Every entity-specific repository inherits from this class and extends it
/// with domain-specific queries.
///
/// Source: Infrastructure layer.
/// Used by: IUnitOfWork and all entity-specific repositories.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T>             _set;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _set     = context.Set<T>();
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _set.FindAsync(new object[] { id }, ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _set.Where(predicate).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _set.FirstOrDefaultAsync(predicate, ct);

    /// <inheritdoc/>
    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        return entity;
    }

    /// <inheritdoc/>
    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _set.AnyAsync(predicate, ct);

    /// <inheritdoc/>
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null
            ? await _set.CountAsync(ct)
            : await _set.CountAsync(predicate, ct);

    /// <inheritdoc/>
    public IQueryable<T> Query() => _set.AsQueryable();
}
