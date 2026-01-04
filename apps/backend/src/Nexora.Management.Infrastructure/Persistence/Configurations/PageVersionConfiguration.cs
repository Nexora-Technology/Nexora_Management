using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class PageVersionConfiguration : IEntityTypeConfiguration<PageVersion>
{
    public void Configure(EntityTypeBuilder<PageVersion> builder)
    {
        builder.ToTable("PageVersions");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pv => pv.PageId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(pv => pv.VersionNumber)
            .IsRequired();

        builder.Property(pv => pv.Content)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'::jsonb");

        builder.Property(pv => pv.CommitMessage)
            .HasColumnType("text");

        builder.Property(pv => pv.CreatedBy)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Indexes
        builder.HasIndex(pv => pv.PageId)
            .HasDatabaseName("idx_page_versions_page");

        builder.HasIndex(pv => new { pv.PageId, pv.VersionNumber })
            .IsUnique()
            .HasDatabaseName("uq_page_versions_page_version");

        builder.HasIndex(pv => pv.CreatedAt)
            .HasDatabaseName("idx_page_versions_created_at");

        // Relationships
        builder.HasOne(pv => pv.Page)
            .WithMany(p => p.Versions)
            .HasForeignKey(pv => pv.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pv => pv.Creator)
            .WithMany()
            .HasForeignKey(pv => pv.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
