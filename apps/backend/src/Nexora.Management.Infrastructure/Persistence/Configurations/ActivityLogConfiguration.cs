using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLog");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.WorkspaceId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntityId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.ChangesJsonb)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(a => new { a.WorkspaceId, a.CreatedAt })
            .IsDescending()
            .HasDatabaseName("idx_activity_workspace");

        builder.HasIndex(a => new { a.EntityType, a.EntityId, a.CreatedAt })
            .IsDescending()
            .HasDatabaseName("idx_activity_entity");

        // Relationships
        builder.HasOne(a => a.Workspace)
            .WithMany(w => w.ActivityLogs)
            .HasForeignKey(a => a.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany(u => u.ActivityLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
