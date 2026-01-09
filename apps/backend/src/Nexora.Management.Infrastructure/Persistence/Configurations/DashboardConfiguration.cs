using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.ToTable("dashboards");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(d => d.WorkspaceId)
            .IsRequired();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Layout)
            .HasColumnType("jsonb");

        builder.Property(d => d.CreatedBy)
            .IsRequired();

        builder.Property(d => d.IsTemplate)
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(d => d.WorkspaceId)
            .HasDatabaseName("idx_dashboards_workspace");

        builder.HasIndex(d => d.CreatedBy)
            .HasDatabaseName("idx_dashboards_created_by");

        builder.HasIndex(d => d.IsTemplate)
            .HasDatabaseName("idx_dashboards_is_template");

        // Relationships
        builder.HasOne(d => d.Workspace)
            .WithMany()
            .HasForeignKey(d => d.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Creator)
            .WithMany()
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
