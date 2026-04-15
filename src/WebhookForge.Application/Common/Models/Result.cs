namespace WebhookForge.Application.Common.Models;

/// <summary>
/// Generic operation result used by all service methods.
/// Replaces exception-throwing with explicit success/failure states that
/// controllers can map directly to HTTP status codes via BaseController.ToActionResult().
/// </summary>
public class Result<T>
{
    public bool    IsSuccess      { get; private set; }
    public T?      Value          { get; private set; }
    public string? Error          { get; private set; }
    public int     StatusCode     { get; private set; }

    // Convenience flags used by BaseController to pick the right IActionResult
    public bool IsNotFound    => StatusCode == 404;
    public bool IsUnauthorized => StatusCode == 401;
    public bool IsForbidden   => StatusCode == 403;

    private Result() { }

    /// <summary>The operation succeeded and produced <paramref name="value"/>.</summary>
    public static Result<T> Success(T value, int statusCode = 200)
        => new() { IsSuccess = true, Value = value, StatusCode = statusCode };

    /// <summary>The operation failed due to a client error.</summary>
    public static Result<T> Failure(string error, int statusCode = 400)
        => new() { IsSuccess = false, Error = error, StatusCode = statusCode };

    /// <summary>The requested resource does not exist.</summary>
    public static Result<T> NotFound(string error = "Resource not found.")
        => Failure(error, 404);

    /// <summary>The caller is not authenticated.</summary>
    public static Result<T> Unauthorized(string error = "Unauthorized.")
        => Failure(error, 401);

    /// <summary>The caller is authenticated but lacks the required permission.</summary>
    public static Result<T> Forbidden(string error = "Access denied.")
        => Failure(error, 403);
}

/// <summary>
/// Non-generic version for operations that return no payload on success (e.g. delete, revoke).
/// </summary>
public class Result
{
    public bool    IsSuccess  { get; private set; }
    public string? Error      { get; private set; }
    public int     StatusCode { get; private set; }

    public bool IsNotFound     => StatusCode == 404;
    public bool IsUnauthorized => StatusCode == 401;
    public bool IsForbidden    => StatusCode == 403;

    private Result() { }

    public static Result Success(int statusCode = 200)
        => new() { IsSuccess = true, StatusCode = statusCode };

    public static Result Failure(string error, int statusCode = 400)
        => new() { IsSuccess = false, Error = error, StatusCode = statusCode };

    public static Result NotFound(string error = "Resource not found.")    => Failure(error, 404);
    public static Result Unauthorized(string error = "Unauthorized.")      => Failure(error, 401);
    public static Result Forbidden(string error = "Access denied.")        => Failure(error, 403);
}
