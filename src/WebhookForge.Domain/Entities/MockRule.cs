using WebhookForge.Domain.Common;

namespace WebhookForge.Domain.Entities;

/// <summary>
/// A rule that is evaluated against each incoming request on a given endpoint.
/// Rules are ordered by <see cref="Priority"/> (ascending — lower = higher priority).
/// The first matching rule wins; its response is returned instead of the default 200 OK.
/// </summary>
public class MockRule : BaseEntity
{
    /// <summary>FK → Endpoints.Id.</summary>
    public Guid EndpointId { get; set; }

    /// <summary>Human-readable name for identifying the rule (max 100 chars).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Evaluation order — lower values are checked first.</summary>
    public int Priority { get; set; }

    /// <summary>
    /// HTTP method to match (GET, POST, etc.).
    /// Null or empty means the rule matches any method.
    /// </summary>
    public string? MatchMethod { get; set; }

    /// <summary>
    /// Regex or exact path to match against the request path.
    /// Null means match any path.
    /// </summary>
    public string? MatchPath { get; set; }

    /// <summary>
    /// Regular expression applied to the raw request body.
    /// Null means no body constraint.
    /// </summary>
    public string? MatchBodyExpression { get; set; }

    /// <summary>HTTP status code to return when this rule matches (100–599).</summary>
    public short ResponseStatus { get; set; } = 200;

    /// <summary>Response body to return verbatim.</summary>
    public string? ResponseBody { get; set; }

    /// <summary>Response headers serialized as JSON Dictionary&lt;string,string&gt;.</summary>
    public string? ResponseHeaders { get; set; }

    /// <summary>Artificial delay in milliseconds before sending the response. Useful for timeout testing.</summary>
    public int DelayMs { get; set; }

    /// <summary>Inactive rules are skipped during evaluation without being deleted.</summary>
    public bool IsActive { get; set; } = true;

    // ── Navigation ──────────────────────────────────────────────
    public WebhookEndpoint Endpoint { get; set; } = null!;
}
