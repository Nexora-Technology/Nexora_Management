using Microsoft.AspNetCore.Authorization;

namespace Nexora.Management.Application.Authorization;

/// <summary>
/// Attribute to require specific permission for endpoint access
/// Usage: [RequirePermission("tasks", "create")]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string resource, string action)
    {
        if (string.IsNullOrWhiteSpace(resource))
            throw new ArgumentException("Resource cannot be empty.", nameof(resource));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty.", nameof(action));

        Policy = $"Permission:{resource}:{action}";
    }
}
