using Microsoft.EntityFrameworkCore;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Infrastructure.Data;

namespace WebhookForge.Infrastructure.Repositories;

/// <summary>
/// User-specific queries on top of the generic repository.
/// Source: Infrastructure layer — implements IUserRepository (Application).
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _set.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await _set.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

    /// <inheritdoc/>
    /// Loads the User navigation so AuthService can build the auth response without an extra query.
    public async Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken ct = default)
        => await _context.Set<RefreshToken>()
            .Include(t => t.User)
            .FirstOrDefaultAsync(
                t => t.Token == token && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow,
                ct);
}
