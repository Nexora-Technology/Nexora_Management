using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.WorkspaceId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasColumnType("text");

        builder.Property(p => p.Color)
            .HasMaxLength(7);

        builder.Property(p => p.Icon)
            .HasMaxLength(50);

        builder.Property(p => p.Status)
            .HasDefaultValue("active")
            .HasMaxLength(20);

        builder.Property(p => p.OwnerId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.SettingsJsonb)
            .HasColumnType("jsonb");

        builder.HasIndex(p => p.WorkspaceId)
            .HasFilter("status = 'active'")
            .HasDatabaseName("idx_projects_workspace");

        builder.HasIndex(p => p.OwnerId)
            .HasDatabaseName("idx_projects_owner");

        builder.HasOne(p => p.Workspace)
            .WithMany(w => w.Projects)
            .HasForeignKey(p => p.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Owner)
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
