using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.ToTable("Spaces");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(s => s.WorkspaceId)
            .IsRequired();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasColumnType("text");

        builder.Property(s => s.Color)
            .HasMaxLength(7);

        builder.Property(s => s.Icon)
            .HasMaxLength(50);

        builder.Property(s => s.IsPrivate)
            .HasDefaultValue(false);

        builder.Property(s => s.SettingsJsonb)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(s => s.WorkspaceId)
            .HasDatabaseName("idx_spaces_workspace");

        // Foreign keys
        builder.HasOne(s => s.Workspace)
            .WithMany(w => w.Spaces)
            .HasForeignKey(s => s.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
