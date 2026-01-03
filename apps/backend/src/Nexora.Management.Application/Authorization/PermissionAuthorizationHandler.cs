using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Authorization;

/// <summary>
/// Authorization requirement for permission-based access control
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Resource { get; }
    public string Action { get; }

    public PermissionRequirement(string resource, string action)
    {
        Resource = resource;
        Action = action;
    }
}

/// <summary>
/// Authorization handler that validates permissions against user roles
/// Supports resource-action based permissions (e.g., tasks:create)
/// Must be registered as Scoped to properly resolve IAppDbContext
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAppDbContext _db;

    public PermissionAuthorizationHandler(IAppDbContext db)
    {
        _db = db;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        // Validate permission format to prevent injection
        if (!IsValidPermissionFormat(requirement.Resource) || !IsValidPermissionFormat(requirement.Action))
        {
            return;
        }

        // Get user's roles and their permissions
        var hasPermission = await _db.SqlQuerySingleAsync<bool>(
            """
            SELECT EXISTS (
                SELECT 1
                FROM "Users" u
                JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
                JOIN "Roles" r ON wm."RoleId" = r."Id"
                JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
                JOIN "Permissions" p ON rp."PermissionId" = p."Id"
                WHERE u."Id" = {0}
                AND p."Name" = {1}
                LIMIT 1
            )
            """,
            userId,
            $"{requirement.Resource}:{requirement.Action}"
        );

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }

    private static bool IsValidPermissionFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            return false;

        // Only allow alphanumeric, colon, hyphen, underscore
        return value.All(c => char.IsLetterOrDigit(c) || c == ':' || c == '-' || c == '_');
    }
}

/// <summary>
/// Dynamic policy provider for permission-based authorization
/// Handles policies in format "Permission:resource:action"
/// </summary>
public class PermissionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "Permission:";
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyPrefix))
        {
            var parts = policyName.Split(':');
            if (parts.Length == 3)
            {
                var resource = parts[1];
                var action = parts[2];

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(resource, action));
                return Task.FromResult(policy.Build());
            }
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
