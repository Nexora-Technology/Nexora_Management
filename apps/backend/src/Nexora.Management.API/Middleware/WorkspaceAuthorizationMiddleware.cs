using System.Security.Claims;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.API.Middleware;

/// <summary>
/// Middleware to set user context for Row-Level Security (RLS)
/// Must be registered after authentication middleware
/// </summary>
public class WorkspaceAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public WorkspaceAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAppDbContext db)
    {
        var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            // Set user context for RLS in PostgreSQL
            // This enables Row-Level Security policies to filter data by user
            await db.ExecuteSqlRawAsync(
                "SET LOCAL app.current_user_id = {0}", userId);
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method to register the middleware
/// </summary>
public static class WorkspaceAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseWorkspaceAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<WorkspaceAuthorizationMiddleware>();
    }
}
