using Microsoft.EntityFrameworkCore;
using Nexora.Management.Domain.Entities;
using TaskEntity = Nexora.Management.Domain.Entities.Task;
using TaskStatusEntity = Nexora.Management.Domain.Entities.TaskStatus;

namespace Nexora.Management.Infrastructure.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkspaceMember> WorkspaceMembers { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<Project> Projects { get; }
    DbSet<TaskStatusEntity> TaskStatuses { get; }
    DbSet<TaskEntity> Tasks { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<ActivityLog> ActivityLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
