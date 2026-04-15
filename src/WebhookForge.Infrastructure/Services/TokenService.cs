using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Entities;
using WebhookForge.Application.Common.Settings;

namespace WebhookForge.Infrastructure.Services;

/// <summary>
/// JWT and refresh-token generation/validation.
/// Source: Infrastructure layer — implements ITokenService (Application).
/// Settings are read from JwtSettings (bound to "Jwt" in appsettings.json).
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;

    public TokenService(IOptions<JwtSettings> jwt) => _jwt = jwt.Value;

    /// <inheritdoc/>
    public string GenerateAccessToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _jwt.Issuer,
            audience:           _jwt.Audience,
            claims:             BuildClaims(user),
            expires:            DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <inheritdoc/>
    public Guid? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var key     = Encoding.UTF8.GetBytes(_jwt.Secret);

        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(key),
                ValidateIssuer           = true,
                ValidIssuer              = _jwt.Issuer,
                ValidateAudience         = true,
                ValidAudience            = _jwt.Audience,
                ValidateLifetime         = false,  // caller decides whether expiry matters
                ClockSkew                = TimeSpan.Zero
            }, out _);

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
        catch { return null; }
    }

    // ── Helpers ──────────────────────────────────────────────────

    private static Claim[] BuildClaims(User user) =>
    [
        new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(JwtRegisteredClaimNames.Name,  user.DisplayName),
        new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
    ];
}
