using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Nexora.Management.Application.Common;

namespace Nexora.Management.API.Middlewares;

/// <summary>
/// Implementation of IUserContext that extracts user info from HttpContext
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User not authenticated");
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?
        .FindFirst(ClaimTypes.Email)?.Value;

    public string? Name => _httpContextAccessor.HttpContext?.User?
        .FindFirst(ClaimTypes.Name)?.Value;
}
