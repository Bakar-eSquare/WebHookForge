namespace WebhookForge.Application.DTOs.Request;

/// <summary>
/// Internal DTO built by WebhookReceiverController from the raw HttpRequest
/// and passed to IRequestService.CaptureAsync.
/// </summary>
public class CaptureRequestDto
{
    /// <summary>Token extracted from the URL path (/hooks/{Token}).</summary>
    public string EndpointToken { get; set; } = string.Empty;

    public string  Method      { get; set; } = string.Empty;
    public string? Path        { get; set; }

    /// <summary>Headers serialized as JSON string by the controller before passing down.</summary>
    public string? Headers     { get; set; }

    public string? QueryString { get; set; }
    public string? Body        { get; set; }
    public string? ContentType { get; set; }
    public string? IpAddress   { get; set; }
}

/// <summary>Captured request representation returned by list and detail endpoints.</summary>
public class IncomingRequestDto
{
    public Guid     Id          { get; set; }
    public Guid     EndpointId  { get; set; }
    public string   Method      { get; set; } = string.Empty;

    /// <summary>Deserialized request headers.</summary>
    public Dictionary<string, string>? Headers { get; set; }

    public string?  Path        { get; set; }
    public string?  QueryString { get; set; }
    public string?  Body        { get; set; }
    public string?  ContentType { get; set; }
    public string?  IpAddress   { get; set; }
    public int      SizeBytes   { get; set; }
    public DateTime ReceivedAt  { get; set; }
}
