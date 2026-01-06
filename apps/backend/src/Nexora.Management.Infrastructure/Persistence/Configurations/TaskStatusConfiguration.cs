using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;
using TaskStatusEntity = Nexora.Management.Domain.Entities.TaskStatus;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatusEntity>
{
    public void Configure(EntityTypeBuilder<TaskStatusEntity> builder)
    {
        builder.ToTable("TaskStatuses");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(ts => ts.ProjectId)
            .HasDefaultValueSql("uuid_generate_v4()");

        // NEW: TaskListId for ClickUp hierarchy migration
        // TODO: After migration, remove ProjectId and keep only TaskListId
        builder.Property(ts => ts.TaskListId)
            .IsRequired();

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.Color)
            .HasMaxLength(7);

        builder.Property(ts => ts.OrderIndex)
            .HasDefaultValue(0);

        builder.Property(ts => ts.Type)
            .HasDefaultValue("open")
            .HasMaxLength(20);

        // Create unique index on ProjectId + OrderIndex to ensure uniqueness per project
        builder.HasIndex(ts => new { ts.ProjectId, ts.OrderIndex })
            .IsUnique();

        // NEW: Unique index on TaskListId + OrderIndex to ensure uniqueness per TaskList
        builder.HasIndex(ts => new { ts.TaskListId, ts.OrderIndex })
            .IsUnique()
            .HasDatabaseName("uq_taskstatuses_tasklist_order");

        // NEW: Index for TaskList (ClickUp hierarchy)
        builder.HasIndex(ts => ts.TaskListId)
            .HasDatabaseName("idx_taskstatuses_tasklist");

        builder.HasOne(ts => ts.Project)
            .WithMany(p => p.TaskStatuses)
            .HasForeignKey(ts => ts.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // NEW: TaskList relationship (ClickUp hierarchy)
        // TODO: After migration, remove Project relationship and keep only TaskList
        builder.HasOne(ts => ts.TaskList)
            .WithMany(tl => tl.TaskStatuses)
            .HasForeignKey(ts => ts.TaskListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
