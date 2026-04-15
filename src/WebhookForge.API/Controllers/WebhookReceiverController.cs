using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using WebhookForge.API.Hubs;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.DTOs.Request;

namespace WebhookForge.API.Controllers;

/// <summary>
/// Public receiver — no authentication required.
/// Accepts any HTTP method on /hooks/{token}, stores the request,
/// evaluates mock rules, and broadcasts via SignalR for live dashboard viewing.
/// </summary>
[ApiController]
[Route("hooks")]
[EnableRateLimiting("webhook")]
public class WebhookReceiverController : ControllerBase
{
    private readonly IRequestService          _requests;
    private readonly IMockRuleService         _mockRules;
    private readonly IHubContext<WebhookHub>  _hub;

    public WebhookReceiverController(
        IRequestService         requests,
        IMockRuleService        mockRules,
        IHubContext<WebhookHub> hub)
    {
        _requests  = requests;
        _mockRules = mockRules;
        _hub       = hub;
    }

    [HttpGet("{token}")]
    [HttpPost("{token}")]
    [HttpPut("{token}")]
    [HttpPatch("{token}")]
    [HttpDelete("{token}")]
    public async Task<IActionResult> Receive(string token, CancellationToken ct)
    {
        // Read body and serialize headers to JSON — both can happen in parallel
        var bodyTask    = ReadBodyAsync(ct);
        var headersJson = JsonSerializer.Serialize(
            Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

        var body = await bodyTask;

        var dto = new CaptureRequestDto
        {
            EndpointToken = token,
            Method        = Request.Method,
            Path          = Request.Path.Value,
            Headers       = headersJson,
            QueryString   = Request.QueryString.Value,
            Body          = body,
            ContentType   = Request.ContentType,
            IpAddress     = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        // Capture + match rule in parallel once we have the endpointId
        var captureResult = await _requests.CaptureAsync(dto, ct);
        if (!captureResult.IsSuccess)
            return NotFound(new { error = captureResult.Error });

        var captured = captureResult.Value!;

        // Match rule and broadcast live feed in parallel
        var ruleTask      = _mockRules.MatchRuleAsync(captured.EndpointId, captured.Method, captured.Path, captured.Body, ct);
        var broadcastTask = _hub.Clients
            .Group(captured.EndpointId.ToString())
            .SendAsync("NewRequest", captured, ct);

        await Task.WhenAll(ruleTask, broadcastTask);

        var rule = ruleTask.Result;
        if (rule is not null)
        {
            foreach (var h in rule.ResponseHeaders ?? new())
                Response.Headers[h.Key] = h.Value;

            if (rule.DelayMs > 0)
                await Task.Delay(rule.DelayMs, ct);

            return StatusCode(rule.ResponseStatus, rule.ResponseBody);
        }

        return Ok(new { received = true, requestId = captured.Id });
    }

    private async Task<string?> ReadBodyAsync(CancellationToken ct)
    {
        if (Request.ContentLength is null or 0) return null;
        using var reader = new System.IO.StreamReader(Request.Body);
        return await reader.ReadToEndAsync(ct);
    }
}
