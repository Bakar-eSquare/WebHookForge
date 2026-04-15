using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Extends the generic repository with user-specific queries.
/// Source: Application layer — implemented in Infrastructure/Repositories/UserRepository.cs.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Find a user by lowercased email address. Returns null if not found.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Returns true if any active user has the given email (case-insensitive).</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Returns the active (non-revoked, non-expired) refresh token record and its associated user.
    /// Used during token rotation to validate the incoming refresh token.
    /// </summary>
    Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken ct = default);
}
