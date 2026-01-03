using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Authentication.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using DomainRefreshToken = Nexora.Management.Domain.Entities.RefreshToken;

namespace Nexora.Management.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IAppDbContext db,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        // Find user
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user == null)
        {
            return Result<AuthResponse>.Failure("Invalid email or password");
        }

        // Verify password
        var result = _passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, request.Password);
        if (result != PasswordVerificationResult.Success)
        {
            return Result<AuthResponse>.Failure("Invalid email or password");
        }

        // Get user roles
        var userRoles = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(ct);

        var roles = userRoles.Any() ? userRoles : new List<string> { "Member" };

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.Name, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Store refresh token
        _db.RefreshTokens.Add(new DomainRefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        });
        await _db.SaveChangesAsync(ct);

        var userDto = new UserDto(user.Id, user.Email, user.Name, user.AvatarUrl);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var authResponse = new AuthResponse(accessToken, refreshToken, expiresAt, userDto);

        return Result<AuthResponse>.Success(authResponse);
    }
}
