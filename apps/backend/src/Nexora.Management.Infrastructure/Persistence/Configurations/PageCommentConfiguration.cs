using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class PageCommentConfiguration : IEntityTypeConfiguration<PageComment>
{
    public void Configure(EntityTypeBuilder<PageComment> builder)
    {
        builder.ToTable("PageComments");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pc => pc.PageId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pc => pc.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pc => pc.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(pc => pc.Selection)
            .HasColumnType("jsonb");

        builder.Property(pc => pc.ParentCommentId)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Indexes
        builder.HasIndex(pc => pc.PageId)
            .HasDatabaseName("idx_page_comments_page");

        builder.HasIndex(pc => pc.UserId)
            .HasDatabaseName("idx_page_comments_user");

        builder.HasIndex(pc => pc.ParentCommentId)
            .HasFilter("parent_comment_id IS NOT NULL")
            .HasDatabaseName("idx_page_comments_parent");

        builder.HasIndex(pc => new { pc.PageId, pc.ParentCommentId })
            .HasDatabaseName("idx_page_comments_page_parent");

        builder.HasIndex(pc => pc.ResolvedAt)
            .HasFilter("resolved_at IS NOT NULL")
            .HasDatabaseName("idx_page_comments_resolved");

        // Self-referencing relationship for nested comments
        builder.HasOne(pc => pc.ParentComment)
            .WithMany(pc => pc.Replies)
            .HasForeignKey(pc => pc.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships
        builder.HasOne(pc => pc.Page)
            .WithMany(p => p.Comments)
            .HasForeignKey(pc => pc.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.User)
            .WithMany()
            .HasForeignKey(pc => pc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
