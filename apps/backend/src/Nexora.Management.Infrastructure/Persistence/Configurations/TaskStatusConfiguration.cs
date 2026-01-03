using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;
using TaskStatusEntity = Nexora.Management.Domain.Entities.TaskStatus;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatusEntity>
{
    public void Configure(EntityTypeBuilder<TaskStatusEntity> builder)
    {
        builder.ToTable("TaskStatuses");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(ts => ts.ProjectId)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ts => ts.Color)
            .HasMaxLength(7);

        builder.Property(ts => ts.OrderIndex)
            .HasDefaultValue(0);

        builder.Property(ts => ts.Type)
            .HasDefaultValue("open")
            .HasMaxLength(20);

        // Create unique index on ProjectId + OrderIndex to ensure uniqueness per project
        builder.HasIndex(ts => new { ts.ProjectId, ts.OrderIndex })
            .IsUnique();

        builder.HasOne(ts => ts.Project)
            .WithMany(p => p.TaskStatuses)
            .HasForeignKey(ts => ts.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
