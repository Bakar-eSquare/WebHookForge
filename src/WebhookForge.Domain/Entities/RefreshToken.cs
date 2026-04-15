using WebhookForge.Domain.Common;

namespace WebhookForge.Domain.Entities;

/// <summary>
/// A long-lived opaque token used to obtain new access tokens without re-authentication.
/// Stored as a BCrypt-safe random base-64 string.
/// Once revoked or expired, a new login is required.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>FK → Users.Id. Cascade-deleted when the user is deleted.</summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Cryptographically random token value (64 bytes → 88 base-64 chars).
    /// Unique across the platform (UX_RefreshTokens_Token).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>UTC expiry — tokens are rejected after this time even if not explicitly revoked.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Set to true on logout or token rotation. Revoked tokens are never reactivated.</summary>
    public bool IsRevoked { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public User User { get; set; } = null!;
}
