using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class PageCollaboratorConfiguration : IEntityTypeConfiguration<PageCollaborator>
{
    public void Configure(EntityTypeBuilder<PageCollaborator> builder)
    {
        builder.ToTable("PageCollaborators");

        // Composite primary key
        builder.HasKey(pc => new { pc.PageId, pc.UserId });

        builder.Property(pc => pc.PageId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pc => pc.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pc => pc.Role)
            .HasDefaultValue("viewer")
            .HasMaxLength(50);

        builder.Property(pc => pc.CreatedAt)
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(pc => pc.PageId)
            .HasDatabaseName("idx_page_collaborators_page");

        builder.HasIndex(pc => pc.UserId)
            .HasDatabaseName("idx_page_collaborators_user");

        // Relationships
        builder.HasOne(pc => pc.Page)
            .WithMany(p => p.Collaborators)
            .HasForeignKey(pc => pc.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.User)
            .WithMany()
            .HasForeignKey(pc => pc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
