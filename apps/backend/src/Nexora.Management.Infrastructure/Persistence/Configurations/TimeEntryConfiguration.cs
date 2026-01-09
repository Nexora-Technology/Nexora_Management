using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("time_entries");

        builder.HasKey(te => te.Id);

        builder.Property(te => te.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(te => te.UserId)
            .IsRequired();

        builder.Property(te => te.TaskId)
            .IsRequired(false);

        builder.Property(te => te.StartTime)
            .IsRequired();

        builder.Property(te => te.EndTime)
            .IsRequired(false);

        builder.Property(te => te.DurationMinutes)
            .IsRequired();

        builder.Property(te => te.Description)
            .HasColumnType("text");

        builder.Property(te => te.IsBillable)
            .HasDefaultValue(false);

        builder.Property(te => te.Status)
            .HasDefaultValue("draft")
            .HasMaxLength(20);

        builder.Property(te => te.WorkspaceId)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(te => te.UserId)
            .HasDatabaseName("idx_time_entries_user");

        builder.HasIndex(te => te.TaskId)
            .HasDatabaseName("idx_time_entries_task");

        builder.HasIndex(te => te.WorkspaceId)
            .HasDatabaseName("idx_time_entries_workspace");

        builder.HasIndex(te => te.StartTime)
            .HasDatabaseName("idx_time_entries_start_time");

        builder.HasIndex(te => te.Status)
            .HasDatabaseName("idx_time_entries_status");

        builder.HasIndex(te => new { te.UserId, te.StartTime })
            .HasDatabaseName("idx_time_entries_user_time");

        // Unique constraint to prevent multiple active timers per user
        // Only one timer can have EndTime = NULL per user
        builder.HasIndex(te => new { te.UserId, te.EndTime })
            .HasFilter("EndTime IS NULL")
            .IsUnique()
            .HasDatabaseName("uq_time_entries_active_timer");

        // Relationships
        builder.HasOne(te => te.User)
            .WithMany()
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(te => te.Task)
            .WithMany()
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(te => te.Workspace)
            .WithMany()
            .HasForeignKey(te => te.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
