using System.Text.Json;

namespace WebhookForge.Application.Common.Helpers;

/// <summary>
/// Shared JSON serialization utilities used across service classes.
/// Centralises the try/catch pattern so individual services don't repeat it.
/// </summary>
public static class JsonHelper
{
    private static readonly JsonSerializerOptions _opts =
        new(JsonSerializerDefaults.Web);   // camelCase, case-insensitive

    /// <summary>
    /// Deserialize <paramref name="json"/> to <typeparamref name="T"/>.
    /// Returns null if the string is empty or deserialization fails.
    /// </summary>
    public static T? TryDeserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try   { return JsonSerializer.Deserialize<T>(json, _opts); }
        catch { return null; }
    }

    /// <summary>
    /// Serialize <paramref name="value"/> to a JSON string.
    /// Returns null when <paramref name="value"/> is null.
    /// </summary>
    public static string? Serialize<T>(T? value) where T : class
        => value is null ? null : JsonSerializer.Serialize(value, _opts);
}
