using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Authentication.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using DomainRefreshToken = Nexora.Management.Domain.Entities.RefreshToken;

namespace Nexora.Management.Application.Authentication.Commands.Register;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IAppDbContext db,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        // Check if user exists
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("User already exists");
        }

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(null!, request.Password),
            Name = request.Name
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // Get Owner role
        var ownerRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Owner", ct);
        if (ownerRole == null)
        {
            return Result<AuthResponse>.Failure("Owner role not found. Please seed roles first.");
        }

        // Create default workspace for user
        var workspace = new Workspace
        {
            Name = $"{user.Name}'s Workspace",
            OwnerId = user.Id
        };
        _db.Workspaces.Add(workspace);
        await _db.SaveChangesAsync(ct);

        // Assign Owner role to user in workspace
        var workspaceMember = new WorkspaceMember
        {
            WorkspaceId = workspace.Id,
            UserId = user.Id,
            RoleId = ownerRole.Id,
            JoinedAt = DateTime.UtcNow
        };
        _db.WorkspaceMembers.Add(workspaceMember);
        await _db.SaveChangesAsync(ct);

        // Generate tokens
        var roles = new List<string> { "Owner" };
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
