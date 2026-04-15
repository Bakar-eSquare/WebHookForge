using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebhookForge.Application.Common.Models;

namespace WebhookForge.API.Controllers;

/// <summary>
/// Base controller providing shared helpers for all API controllers:
/// - CurrentUserId extracted from the JWT 'sub' claim.
/// - ToActionResult overloads that map Result/Result&lt;T&gt; to the correct HTTP status.
/// Controllers should stay thin: validate input, call service, return ToActionResult.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// The authenticated user's ID, parsed from the JWT 'sub' claim.
    /// Returns Guid.Empty when the user is not authenticated.
    /// </summary>
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    /// <summary>Maps a typed Result to the appropriate IActionResult.</summary>
    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)        return Ok(result.Value);
        if (result.IsNotFound)       return NotFound(new  { error = result.Error });
        if (result.IsUnauthorized)   return Unauthorized(new { error = result.Error });
        if (result.IsForbidden)      return StatusCode(403, new { error = result.Error });
        return BadRequest(new { error = result.Error });
    }

    /// <summary>Maps a non-generic Result (delete/revoke) to the appropriate IActionResult.</summary>
    protected IActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)        return NoContent();
        if (result.IsNotFound)       return NotFound(new  { error = result.Error });
        if (result.IsUnauthorized)   return Unauthorized(new { error = result.Error });
        if (result.IsForbidden)      return StatusCode(403, new { error = result.Error });
        return BadRequest(new { error = result.Error });
    }

    /// <summary>Maps a 201-Created result and sets the Location header via route name.</summary>
    protected IActionResult ToCreatedResult<T>(Result<T> result, string routeName, object routeValues)
    {
        if (!result.IsSuccess) return ToActionResult(result);
        return CreatedAtRoute(routeName, routeValues, result.Value);
    }
}
