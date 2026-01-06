using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.ToTable("Folders");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(f => f.SpaceId)
            .IsRequired();

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasColumnType("text");

        builder.Property(f => f.Color)
            .HasMaxLength(7);

        builder.Property(f => f.Icon)
            .HasMaxLength(50);

        builder.Property(f => f.PositionOrder)
            .HasDefaultValue(0);

        builder.Property(f => f.SettingsJsonb)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(f => f.SpaceId)
            .HasDatabaseName("idx_folders_space");

        // Unique index for drag-and-drop ordering (SpaceId + PositionOrder must be unique)
        builder.HasIndex(f => new { f.SpaceId, f.PositionOrder })
            .IsUnique()
            .HasDatabaseName("uq_folders_space_position");

        // Foreign keys
        builder.HasOne(f => f.Space)
            .WithMany(s => s.Folders)
            .HasForeignKey(f => f.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
