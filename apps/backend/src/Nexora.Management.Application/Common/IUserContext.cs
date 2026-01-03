using System.Security.Claims;

namespace Nexora.Management.Application.Common;

/// <summary>
/// Interface to access current user context from requests
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the current user ID from JWT claims
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets the current user email from JWT claims
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user name from JWT claims
    /// </summary>
    string? Name { get; }
}
