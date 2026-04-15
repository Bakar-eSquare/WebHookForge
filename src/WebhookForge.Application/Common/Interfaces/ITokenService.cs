using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Common.Interfaces;

/// <summary>
/// Generates and validates JWT access tokens and opaque refresh tokens.
/// Source: Application layer — implemented in Infrastructure/Services/TokenService.cs.
/// </summary>
public interface ITokenService
{
    /// <summary>Generates a signed JWT access token for the given user. Expiry is read from JwtSettings.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Generates a cryptographically random 64-byte refresh token encoded as base-64.</summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates the token's signature, issuer, and audience (ignores expiry).
    /// Returns the user ID from the 'sub' claim, or null if the token is invalid.
    /// </summary>
    Guid? GetUserIdFromToken(string token);
}
