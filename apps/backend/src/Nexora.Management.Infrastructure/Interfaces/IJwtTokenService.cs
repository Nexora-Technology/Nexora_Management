using System.Security.Claims;

namespace Nexora.Management.Infrastructure.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, string? name, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
