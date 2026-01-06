using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class TaskListConfiguration : IEntityTypeConfiguration<TaskList>
{
    public void Configure(EntityTypeBuilder<TaskList> builder)
    {
        builder.ToTable("TaskLists");

        builder.HasKey(tl => tl.Id);

        builder.Property(tl => tl.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(tl => tl.SpaceId)
            .IsRequired();

        builder.Property(tl => tl.FolderId)
            .IsRequired(false);

        builder.Property(tl => tl.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tl => tl.Description)
            .HasColumnType("text");

        builder.Property(tl => tl.Color)
            .HasMaxLength(7);

        builder.Property(tl => tl.Icon)
            .HasMaxLength(50);

        builder.Property(tl => tl.ListType)
            .HasDefaultValue("task")
            .HasMaxLength(50);

        builder.Property(tl => tl.Status)
            .HasDefaultValue("active")
            .HasMaxLength(20);

        builder.Property(tl => tl.OwnerId)
            .IsRequired();

        builder.Property(tl => tl.PositionOrder)
            .HasDefaultValue(0);

        builder.Property(tl => tl.SettingsJsonb)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(tl => tl.SpaceId)
            .HasDatabaseName("idx_tasklists_space");

        builder.HasIndex(tl => tl.FolderId)
            .HasDatabaseName("idx_tasklists_folder");

        builder.HasIndex(tl => new { tl.SpaceId, tl.FolderId, tl.PositionOrder })
            .HasDatabaseName("idx_tasklists_position");

        // Filtered index for active lists
        builder.HasIndex(tl => tl.SpaceId)
            .HasFilter("status = 'active'")
            .HasDatabaseName("idx_tasklists_space_active");

        // Foreign keys
        builder.HasOne(tl => tl.Space)
            .WithMany(s => s.TaskLists)
            .HasForeignKey(tl => tl.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tl => tl.Folder)
            .WithMany(f => f.TaskLists)
            .HasForeignKey(tl => tl.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tl => tl.Owner)
            .WithMany(u => u.OwnedTaskLists) // NEW: OwnedTaskLists navigation
            .HasForeignKey(tl => tl.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
