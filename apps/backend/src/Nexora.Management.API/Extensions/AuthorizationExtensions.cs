using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexora.Management.Application.Authorization;

namespace Nexora.Management.API.Extensions;

/// <summary>
/// Extension methods for applying authorization to minimal API endpoints
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Applies permission-based authorization to an endpoint
    /// Usage: .RequirePermission("tasks", "create")
    /// </summary>
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string resource,
        string action)
    {
        var policyName = $"Permission:{resource}:{action}";
        return builder.RequireAuthorization(policyName);
    }
}
