using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.WorkspaceId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Icon)
            .HasMaxLength(100);

        builder.Property(p => p.CoverImage)
            .HasMaxLength(1000);

        builder.Property(p => p.Content)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb");

        builder.Property(p => p.ContentType)
            .HasDefaultValue("rich-text")
            .HasMaxLength(50);

        builder.Property(p => p.Status)
            .HasDefaultValue("active")
            .HasMaxLength(50);

        builder.Property(p => p.IsFavorite)
            .HasDefaultValue(false);

        builder.Property(p => p.PositionOrder)
            .HasDefaultValue(0);

        builder.Property(p => p.CreatedBy)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(p => p.UpdatedBy)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Indexes
        builder.HasIndex(p => p.WorkspaceId)
            .HasDatabaseName("idx_pages_workspace");

        builder.HasIndex(p => p.ParentPageId)
            .HasFilter("parent_page_id IS NOT NULL")
            .HasDatabaseName("idx_pages_parent");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("idx_pages_status");

        builder.HasIndex(p => new { p.WorkspaceId, p.ParentPageId })
            .HasDatabaseName("idx_pages_workspace_parent");

        builder.HasIndex(p => p.Slug)
            .HasDatabaseName("idx_pages_slug");

        builder.HasIndex(p => p.Content)
            .HasMethod("gin")
            .HasDatabaseName("idx_pages_content");

        builder.HasIndex(p => new { p.WorkspaceId, p.Status, p.IsFavorite })
            .HasDatabaseName("idx_pages_workspace_status_favorite");

        // Unique constraint per workspace for slug
        builder.HasIndex(p => new { p.WorkspaceId, p.Slug })
            .IsUnique()
            .HasDatabaseName("uq_pages_workspace_slug");

        // Self-referencing relationship for hierarchy
        builder.HasOne(p => p.ParentPage)
            .WithMany(p => p.SubPages)
            .HasForeignKey(p => p.ParentPageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Workspace)
            .WithMany()
            .HasForeignKey(p => p.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Updater)
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
