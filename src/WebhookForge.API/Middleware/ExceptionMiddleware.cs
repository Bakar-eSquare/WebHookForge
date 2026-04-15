using System.Net;
using System.Text.Json;

namespace WebhookForge.API.Middleware;

/// <summary>
/// Last-resort safety net that catches any unhandled exception, logs it,
/// and returns a clean JSON 500 instead of leaking a stack trace.
/// The Result&lt;T&gt; pattern handles expected errors — this catches everything else
/// (DB timeouts, null refs, serialization failures, etc.).
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate             _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment            _env;

    public ExceptionMiddleware(
        RequestDelegate              next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment             env)
    {
        _next   = next;
        _logger = logger;
        _env    = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
                return; // Can't write headers after streaming has begun

            context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Expose the real message in dev only — never leak internals in production
            var message = _env.IsDevelopment()
                ? ex.Message
                : "An unexpected error occurred. Please try again later.";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new { error = message }));
        }
    }
}
