using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class TimeRateConfiguration : IEntityTypeConfiguration<TimeRate>
{
    public void Configure(EntityTypeBuilder<TimeRate> builder)
    {
        builder.ToTable("time_rates");

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(tr => tr.UserId)
            .IsRequired(false);

        builder.Property(tr => tr.ProjectId)
            .IsRequired(false);

        builder.Property(tr => tr.HourlyRate)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(tr => tr.Currency)
            .HasDefaultValue("USD")
            .HasMaxLength(3);

        builder.Property(tr => tr.EffectiveFrom)
            .IsRequired();

        builder.Property(tr => tr.EffectiveTo)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(tr => tr.UserId)
            .HasDatabaseName("idx_time_rates_user");

        builder.HasIndex(tr => tr.ProjectId)
            .HasDatabaseName("idx_time_rates_project");

        builder.HasIndex(tr => new { tr.UserId, tr.EffectiveFrom })
            .HasDatabaseName("idx_time_rates_user_effective");

        builder.HasIndex(tr => new { tr.ProjectId, tr.EffectiveFrom })
            .HasDatabaseName("idx_time_rates_project_effective");

        // Relationships
        builder.HasOne(tr => tr.User)
            .WithMany()
            .HasForeignKey(tr => tr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tr => tr.Project)
            .WithMany()
            .HasForeignKey(tr => tr.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
