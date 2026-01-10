using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.Tests.Helpers;

public class TestDataBuilder
{
    private readonly AppDbContext _dbContext;

    public TestDataBuilder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User CreateUser(string? email = null, string? name = null)
    {
        var user = new User
        {
            Email = email ?? $"test{Guid.NewGuid()}@example.com",
            Name = name ?? "Test User",
            PasswordHash = "hashed_password"
        };

        _dbContext.Users.Add(user);
        return user;
    }

    public Workspace CreateWorkspace(Guid ownerId, string? name = null)
    {
        var workspace = new Workspace
        {
            Name = name ?? $"Test Workspace {Guid.NewGuid()}",
            OwnerId = ownerId
        };

        _dbContext.Workspaces.Add(workspace);
        return workspace;
    }

    public Role CreateRole(string name, string? description = null)
    {
        var role = new Role
        {
            Name = name,
            Description = description ?? $"Test role {name}"
        };

        _dbContext.Roles.Add(role);
        return role;
    }

    public Permission CreatePermission(string resource, string action, string? description = null)
    {
        var permission = new Permission
        {
            Resource = resource,
            Action = action,
            Description = description ?? $"Test permission for {resource}:{action}"
        };

        _dbContext.Permissions.Add(permission);
        return permission;
    }

    public WorkspaceMember AddWorkspaceMember(Guid workspaceId, Guid userId, Guid roleId)
    {
        var member = new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            RoleId = roleId
        };

        _dbContext.WorkspaceMembers.Add(member);
        return member;
    }

    public async Task<User> CreateSavedUserAsync(string? email = null, string? name = null)
    {
        var user = CreateUser(email, name);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> CreateSavedUserAsync(string email, string name, string plainPassword)
    {
        var user = new User
        {
            Email = email,
            Name = name,
            PasswordHash = $"HASHED_{plainPassword}" // Mock hash for testing
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<Role> CreateSavedRoleAsync(string name, string? description = null)
    {
        var role = CreateRole(name, description);
        await _dbContext.SaveChangesAsync();
        return role;
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        _dbContext.UserRoles.Add(userRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Workspace> CreateSavedWorkspaceAsync(Guid ownerId, string? name = null)
    {
        var workspace = CreateWorkspace(ownerId, name);
        await _dbContext.SaveChangesAsync();
        return workspace;
    }
}
