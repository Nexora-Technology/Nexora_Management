using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(c => c.TaskId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(c => c.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(c => c.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.ParentCommentId)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Indexes
        builder.HasIndex(c => c.TaskId)
            .HasDatabaseName("idx_comments_task");

        builder.HasIndex(c => c.ParentCommentId)
            .HasFilter("parent_comment_id IS NOT NULL")
            .HasDatabaseName("idx_comments_parent");

        // Relationships
        builder.HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
