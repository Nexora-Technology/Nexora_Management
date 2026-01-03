using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.OwnerId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(w => w.SettingsJsonb)
            .HasColumnType("jsonb");

        builder.HasIndex(w => w.OwnerId)
            .HasDatabaseName("idx_workspaces_owner");

        builder.HasOne(w => w.Owner)
            .WithMany(u => u.OwnedWorkspaces)
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
