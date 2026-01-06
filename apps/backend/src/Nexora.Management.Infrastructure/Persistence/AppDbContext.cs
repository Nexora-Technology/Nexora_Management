using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nexora.Management.Domain.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using TaskEntity = Nexora.Management.Domain.Entities.Task;
using TaskStatusEntity = Nexora.Management.Domain.Entities.TaskStatus;

namespace Nexora.Management.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ignore the pending model changes warning during migration
        // In development, this is expected when adding new migrations
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Space> Spaces => Set<Space>(); // NEW: ClickUp hierarchy
    public DbSet<Folder> Folders => Set<Folder>(); // NEW: ClickUp hierarchy
    public DbSet<TaskList> TaskLists => Set<TaskList>(); // NEW: ClickUp hierarchy (Lists)
    public DbSet<TaskStatusEntity> TaskStatuses => Set<TaskStatusEntity>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<UserPresence> UserPresences => Set<UserPresence>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageVersion> PageVersions => Set<PageVersion>();
    public DbSet<PageComment> PageComments => Set<PageComment>();
    public DbSet<GoalPeriod> GoalPeriods => Set<GoalPeriod>();
    public DbSet<Objective> Objectives => Set<Objective>();
    public DbSet<KeyResult> KeyResults => Set<KeyResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Setup PostgreSQL extensions
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.HasPostgresExtension("pg_trgm");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                if (entry.Entity.Id == Guid.Empty)
                {
                    entry.Entity.Id = Guid.NewGuid();
                }
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task<List<T>> SqlQueryRawAsync<T>(string sql, params object[] parameters)
    {
        return await Database.SqlQueryRaw<T>(sql, parameters).ToListAsync();
    }

    public async Task<T> SqlQuerySingleAsync<T>(string sql, params object[] parameters)
    {
        return await Database.SqlQueryRaw<T>(sql, parameters).FirstOrDefaultAsync();
    }
}
