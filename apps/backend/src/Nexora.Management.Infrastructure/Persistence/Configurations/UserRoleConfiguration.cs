using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId, ur.WorkspaceId });

        builder.Property(ur => ur.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(ur => ur.RoleId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(ur => ur.WorkspaceId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Workspace)
            .WithMany(w => w.UserRoles)
            .HasForeignKey(ur => ur.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
