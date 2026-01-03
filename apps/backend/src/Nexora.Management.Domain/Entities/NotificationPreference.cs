using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

/// <summary>
/// User notification preferences
/// </summary>
public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    // Notification type preferences
    public bool TaskAssignedEnabled { get; set; } = true;
    public bool CommentMentionedEnabled { get; set; } = true;
    public bool StatusChangedEnabled { get; set; } = true;
    public bool DueDateReminderEnabled { get; set; } = true;
    public bool ProjectInvitationEnabled { get; set; } = true;

    // Delivery channel preferences
    public bool InAppEnabled { get; set; } = true;
    public bool EmailEnabled { get; set; } = true;
    public bool BrowserNotificationEnabled { get; set; } = false;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
