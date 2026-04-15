using System.Linq.Expressions;
using WebhookForge.Domain.Common;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Generic read/write repository contract.
/// All entity-specific repositories extend this interface and add domain-specific queries.
/// Use <see cref="Query"/> to compose arbitrary LINQ queries via EF Core when the
/// pre-defined methods are insufficient.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Returns the entity with the given primary key, or null if not found.</summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns all entities matching the predicate.</summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>Returns the first entity matching the predicate, or null.</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>Stages the entity for INSERT. Call <see cref="IUnitOfWork.SaveChangesAsync"/> to persist.</summary>
    Task<T> AddAsync(T entity, CancellationToken ct = default);

    /// <summary>Stages the entity for UPDATE. Call <see cref="IUnitOfWork.SaveChangesAsync"/> to persist.</summary>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>Stages the entity for DELETE. Call <see cref="IUnitOfWork.SaveChangesAsync"/> to persist.</summary>
    Task DeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>Returns true if at least one entity satisfies the predicate.</summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>Returns the count of entities matching the predicate, or total count when null.</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

    /// <summary>
    /// Exposes the underlying <see cref="IQueryable{T}"/> for building complex queries
    /// with Include, OrderBy, GroupBy, etc. before materialising with ToListAsync.
    /// </summary>
    IQueryable<T> Query();
}
