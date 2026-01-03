using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("WorkspaceMembers");

        builder.HasKey(wm => new { wm.WorkspaceId, wm.UserId });

        builder.Property(wm => wm.WorkspaceId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(wm => wm.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(wm => wm.RoleId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(wm => wm.JoinedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(wm => wm.UserId)
            .HasDatabaseName("idx_workspace_members_user");

        builder.HasIndex(wm => wm.WorkspaceId)
            .HasDatabaseName("idx_workspace_members_workspace");

        builder.HasOne(wm => wm.Workspace)
            .WithMany(w => w.Members)
            .HasForeignKey(wm => wm.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wm => wm.User)
            .WithMany(u => u.WorkspaceMemberships)
            .HasForeignKey(wm => wm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wm => wm.Role)
            .WithMany()
            .HasForeignKey(wm => wm.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
