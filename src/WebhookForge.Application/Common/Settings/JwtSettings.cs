namespace WebhookForge.Application.Common.Settings;

/// <summary>
/// JWT configuration — bound from the "Jwt" section in appsettings.json.
/// Lives in the Application layer so AuthService can read expiry values
/// without taking a dependency on Infrastructure.
/// </summary>
public class JwtSettings
{
    public string Secret                   { get; set; } = string.Empty;
    public string Issuer                   { get; set; } = string.Empty;
    public string Audience                 { get; set; } = string.Empty;
    public int    AccessTokenExpiryMinutes { get; set; } = 15;
    public int    RefreshTokenExpiryDays   { get; set; } = 30;
}
