using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nexora.Management.Application.Authentication.Commands.Register;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Tests.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.Tests.Application.Authentication;

public class RegisterCommandTests : TestBase
{
    private readonly TestDataBuilder _dataBuilder;

    public RegisterCommandTests()
    {
        _dataBuilder = new TestDataBuilder(DbContext);
    }

    [Fact]
    public async Task UserEntity_CanBeCreated()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "newuser@example.com",
            Name = "New User",
            PasswordHash = "hashed_password"
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        // Assert
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be("newuser@example.com");
        user.Name.Should().Be("New User");
    }

    [Fact]
    public async Task WorkspaceEntity_CanBeCreated()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            OwnerId = ownerId
        };

        DbContext.Workspaces.Add(workspace);
        await DbContext.SaveChangesAsync();

        // Assert
        workspace.Id.Should().NotBeEmpty();
        workspace.Name.Should().Be("Test Workspace");
        workspace.OwnerId.Should().Be(ownerId);
    }

    [Fact]
    public async Task WorkspaceMember_CanBeCreated()
    {
        // Arrange
        var user = await _dataBuilder.CreateSavedUserAsync();
        var workspace = await _dataBuilder.CreateSavedWorkspaceAsync(user.Id);
        var role = _dataBuilder.CreateRole("Owner");
        await DbContext.SaveChangesAsync();

        // Act
        var member = new WorkspaceMember
        {
            WorkspaceId = workspace.Id,
            UserId = user.Id,
            RoleId = role.Id,
            JoinedAt = DateTime.UtcNow
        };

        DbContext.WorkspaceMembers.Add(member);
        await DbContext.SaveChangesAsync();

        // Assert
        member.Id.Should().NotBeEmpty();
        member.WorkspaceId.Should().Be(workspace.Id);
        member.UserId.Should().Be(user.Id);
        member.RoleId.Should().Be(role.Id);
    }

    [Fact]
    public async Task RefreshToken_CanBeCreated()
    {
        // Arrange
        var user = await _dataBuilder.CreateSavedUserAsync();

        // Act
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        DbContext.RefreshTokens.Add(refreshToken);
        await DbContext.SaveChangesAsync();

        // Assert
        refreshToken.Id.Should().NotBeEmpty();
        refreshToken.UserId.Should().Be(user.Id);
        refreshToken.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RoleEntity_CanBeCreated()
    {
        // Arrange & Act
        var role = new Role
        {
            Name = "Admin",
            Description = "Administrator role"
        };

        DbContext.Roles.Add(role);
        await DbContext.SaveChangesAsync();

        // Assert
        role.Id.Should().NotBeEmpty();
        role.Name.Should().Be("Admin");
        role.Description.Should().Be("Administrator role");
    }

    [Fact]
    public async Task PermissionEntity_CanBeCreated()
    {
        // Arrange & Act
        var permission = new Permission
        {
            Resource = "tasks",
            Action = "create",
            Description = "Create tasks"
        };

        DbContext.Permissions.Add(permission);
        await DbContext.SaveChangesAsync();

        // Assert
        permission.Id.Should().NotBeEmpty();
        permission.Resource.Should().Be("tasks");
        permission.Action.Should().Be("create");
        permission.Description.Should().Be("Create tasks");
    }
}
