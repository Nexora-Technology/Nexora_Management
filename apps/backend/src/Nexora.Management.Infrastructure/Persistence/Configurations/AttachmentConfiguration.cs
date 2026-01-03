using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.TaskId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.UserId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(a => a.FileSizeBytes)
            .HasColumnType("bigint");

        builder.Property(a => a.MimeType)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(a => a.TaskId)
            .HasDatabaseName("idx_attachments_task");

        // Relationships
        builder.HasOne(a => a.Task)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Attachments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
