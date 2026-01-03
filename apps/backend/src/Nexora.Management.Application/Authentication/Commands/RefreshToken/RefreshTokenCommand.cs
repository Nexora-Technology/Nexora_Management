using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Authentication.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using DomainRefreshToken = Nexora.Management.Domain.Entities.RefreshToken;

namespace Nexora.Management.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IAppDbContext _db;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IAppDbContext db,
        IJwtTokenService jwtTokenService)
    {
        _db = db;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        // Validate access token to get user info
        var principal = _jwtTokenService.ValidateToken(request.AccessToken);
        if (principal == null)
        {
            return Result<AuthResponse>.Failure("Invalid access token");
        }

        var userIdStr = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Result<AuthResponse>.Failure("Invalid token");
        }

        // Find refresh token
        var refreshTokenEntity = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId, ct);

        if (refreshTokenEntity == null)
        {
            return Result<AuthResponse>.Failure("Invalid refresh token");
        }

        if (refreshTokenEntity.IsUsed || refreshTokenEntity.IsRevoked)
        {
            return Result<AuthResponse>.Failure("Refresh token has been used or revoked");
        }

        if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            return Result<AuthResponse>.Failure("Refresh token has expired");
        }

        // Mark old refresh token as used
        refreshTokenEntity.IsUsed = true;
        await _db.SaveChangesAsync(ct);

        // Get user roles
        var userRoles = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(ct);

        var roles = userRoles.Any() ? userRoles : new List<string> { "Member" };
        var user = refreshTokenEntity.User;

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.Name, roles);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store new refresh token
        _db.RefreshTokens.Add(new DomainRefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        });
        await _db.SaveChangesAsync(ct);

        var userDto = new UserDto(user.Id, user.Email, user.Name, user.AvatarUrl);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var authResponse = new AuthResponse(newAccessToken, newRefreshToken, expiresAt, userDto);

        return Result<AuthResponse>.Success(authResponse);
    }
}
