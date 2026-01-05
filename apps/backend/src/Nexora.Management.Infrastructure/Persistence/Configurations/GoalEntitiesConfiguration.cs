using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class GoalPeriodConfiguration : IEntityTypeConfiguration<GoalPeriod>
{
    public void Configure(EntityTypeBuilder<GoalPeriod> builder)
    {
        builder.ToTable("goal_periods");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(g => g.WorkspaceId)
            .IsRequired();

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Status)
            .HasDefaultValue("active")
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(g => g.WorkspaceId);

        // Relationships
        builder.HasOne(g => g.Workspace)
            .WithMany()
            .HasForeignKey(g => g.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
{
    public void Configure(EntityTypeBuilder<Objective> builder)
    {
        builder.ToTable("objectives");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(o => o.WorkspaceId)
            .IsRequired();

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Description)
            .HasColumnType("text");

        builder.Property(o => o.Status)
            .HasDefaultValue("on-track")
            .HasMaxLength(20);

        // Computed progress column - will be updated by application logic
        builder.Property(o => o.Progress)
            .HasDefaultValue(0);

        builder.Property(o => o.Weight)
            .HasDefaultValue(1);

        builder.Property(o => o.PositionOrder)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(o => o.WorkspaceId);
        builder.HasIndex(o => new { o.WorkspaceId, o.ParentObjectiveId });
        builder.HasIndex(o => new { o.WorkspaceId, o.Status });

        // Self-referencing relationship (hierarchy)
        builder.HasOne(o => o.ParentObjective)
            .WithMany(o => o.SubObjectives)
            .HasForeignKey(o => o.ParentObjectiveId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships
        builder.HasOne(o => o.Workspace)
            .WithMany()
            .HasForeignKey(o => o.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Period)
            .WithMany(p => p.Objectives)
            .HasForeignKey(o => o.PeriodId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Owner)
            .WithMany()
            .HasForeignKey(o => o.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class KeyResultConfiguration : IEntityTypeConfiguration<KeyResult>
{
    public void Configure(EntityTypeBuilder<KeyResult> builder)
    {
        builder.ToTable("key_results");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(k => k.ObjectiveId)
            .IsRequired();

        builder.Property(k => k.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(k => k.MetricType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(k => k.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(k => k.Progress)
            .HasDefaultValue(0);

        builder.Property(k => k.Weight)
            .HasDefaultValue(1);

        // Indexes
        builder.HasIndex(k => k.ObjectiveId);
        builder.HasIndex(k => new { k.ObjectiveId, k.DueDate });

        // Relationship
        builder.HasOne(k => k.Objective)
            .WithMany(o => o.KeyResults)
            .HasForeignKey(k => k.ObjectiveId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
