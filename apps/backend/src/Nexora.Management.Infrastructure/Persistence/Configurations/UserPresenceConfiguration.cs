using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class UserPresenceConfiguration : IEntityTypeConfiguration<UserPresence>
{
    public void Configure(EntityTypeBuilder<UserPresence> builder)
    {
        builder.ToTable("user_presence");

        builder.HasKey(up => up.Id);
        builder.Property(up => up.Id).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(up => up.UserId).IsRequired();
        builder.Property(up => up.WorkspaceId).IsRequired();
        builder.Property(up => up.ConnectionId).HasMaxLength(255);
        builder.Property(up => up.LastSeen).IsRequired();
        builder.Property(up => up.IsOnline).IsRequired();

        // JSONB column for metadata
        builder.Property(up => up.Metadata)
            .HasColumnType("jsonb");

        // Unique constraint: one presence record per user per workspace
        builder.HasIndex(up => new { up.UserId, up.WorkspaceId })
            .IsUnique();

        // Index for finding active users in workspace
        builder.HasIndex(up => up.WorkspaceId);

        // Index for cleaning up stale connections
        builder.HasIndex(up => up.LastSeen);

        // Index for connection lookup
        builder.HasIndex(up => up.ConnectionId);

        // Relationships
        builder.HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Workspace)
            .WithMany()
            .HasForeignKey(up => up.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
