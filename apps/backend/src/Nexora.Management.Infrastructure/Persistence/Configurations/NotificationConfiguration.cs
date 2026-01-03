using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(n => n.UserId).IsRequired();
        builder.Property(n => n.WorkspaceId);
        builder.Property(n => n.Type).IsRequired().HasMaxLength(100);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(500);
        builder.Property(n => n.Message).HasMaxLength(2000);
        builder.Property(n => n.ActionUrl).HasMaxLength(1000);
        builder.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
        builder.Property(n => n.ReadAt);

        // JSONB column for metadata
        builder.Property(n => n.Metadata)
            .HasColumnType("jsonb");

        // Index for querying user's unread notifications
        builder.HasIndex(n => new { n.UserId, n.IsRead });

        // Index for querying user's notifications by date
        builder.HasIndex(n => new { n.UserId, n.CreatedAt });

        // Index for workspace-scoped notifications
        builder.HasIndex(n => n.WorkspaceId);

        // Relationships
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Workspace)
            .WithMany()
            .HasForeignKey(n => n.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
