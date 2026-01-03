using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;
using TaskEntity = Nexora.Management.Domain.Entities.Task;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(t => t.ProjectId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasColumnType("text");

        builder.Property(t => t.Priority)
            .HasDefaultValue("medium")
            .HasMaxLength(20);

        builder.Property(t => t.EstimatedHours)
            .HasColumnType("decimal(5,2)");

        builder.Property(t => t.PositionOrder)
            .HasDefaultValue(0);

        builder.Property(t => t.CustomFieldsJsonb)
            .HasColumnType("jsonb");

        builder.Property(t => t.CreatedBy)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Indexes
        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("idx_tasks_project");

        builder.HasIndex(t => t.StatusId)
            .HasDatabaseName("idx_tasks_status");

        builder.HasIndex(t => t.AssigneeId)
            .HasFilter("assignee_id IS NOT NULL")
            .HasDatabaseName("idx_tasks_assignee");

        builder.HasIndex(t => t.ParentTaskId)
            .HasFilter("parent_task_id IS NOT NULL")
            .HasDatabaseName("idx_tasks_parent");

        builder.HasIndex(t => t.DueDate)
            .HasFilter("due_date IS NOT NULL")
            .HasDatabaseName("idx_tasks_due_date");

        builder.HasIndex(t => new { t.ProjectId, t.StatusId, t.PositionOrder })
            .HasDatabaseName("idx_tasks_list");

        builder.HasIndex(t => t.CustomFieldsJsonb)
            .HasMethod("gin")
            .HasDatabaseName("idx_tasks_custom_fields");

        // Relationships
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.Subtasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Creator)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Status)
            .WithMany(ts => ts.Tasks)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
