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
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Project> Projects { get; }
    DbSet<TaskStatusEntity> TaskStatuses { get; }
    DbSet<TaskEntity> Tasks { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<ActivityLog> ActivityLogs { get; }
    DbSet<UserPresence> UserPresences { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<Page> Pages { get; }
    DbSet<PageVersion> PageVersions { get; }
    DbSet<PageComment> PageComments { get; }
    DbSet<GoalPeriod> GoalPeriods { get; }
    DbSet<Objective> Objectives { get; }
    DbSet<KeyResult> KeyResults { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // For raw SQL execution (needed for RLS and authorization queries)
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);

    // For raw SQL queries with return type
    Task<List<T>> SqlQueryRawAsync<T>(string sql, params object[] parameters);

    // For raw SQL query that returns single result
    Task<T> SqlQuerySingleAsync<T>(string sql, params object[] parameters);
}
