using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Management.Domain.Entities;

namespace Nexora.Management.Infrastructure.Persistence.Configurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.HasKey(np => np.Id);
        builder.Property(np => np.Id).HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(np => np.UserId).IsRequired();

        // Notification type preferences
        builder.Property(np => np.TaskAssignedEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(np => np.CommentMentionedEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(np => np.StatusChangedEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(np => np.DueDateReminderEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(np => np.ProjectInvitationEnabled).IsRequired().HasDefaultValue(true);

        // Delivery channel preferences
        builder.Property(np => np.InAppEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(np => np.EmailEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(np => np.BrowserNotificationEnabled).IsRequired().HasDefaultValue(false);

        builder.Property(np => np.UpdatedAt).IsRequired().HasDefaultValueSql("NOW()");

        // Unique index on UserId
        builder.HasIndex(np => np.UserId).IsUnique();

        // Relationship
        builder.HasOne(np => np.User)
            .WithMany()
            .HasForeignKey(np => np.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
