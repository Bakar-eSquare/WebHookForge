namespace WebhookForge.Application.DTOs.Auth;

/// <summary>Returned by register, login, and token-refresh endpoints.</summary>
public class AuthResponseDto
{
    /// <summary>Short-lived JWT — include as Bearer token in Authorization header.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Long-lived opaque token used to obtain new access tokens.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>UTC expiry of the access token.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Basic profile of the authenticated user.</summary>
    public UserInfoDto User { get; set; } = null!;
}

/// <summary>Lightweight user info embedded in auth responses and the /auth/me endpoint.</summary>
public class UserInfoDto
{
    public Guid   Id          { get; set; }
    public string Email       { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>Request body for POST /api/auth/refresh.</summary>
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>Request body for POST /api/auth/revoke.</summary>
public class RevokeTokenDto
{
    public string Token { get; set; } = string.Empty;
}
