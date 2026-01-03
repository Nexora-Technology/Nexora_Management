namespace Nexora.Management.Infrastructure.Interfaces;

using Nexora.Management.Domain.Entities;

/// <summary>
/// Service for managing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Create and send a notification to a user
    /// </summary>
    System.Threading.Tasks.Task<Notification> CreateNotificationAsync(
        Guid userId,
        string type,
        string title,
        string? message = null,
        string? actionUrl = null,
        Guid? workspaceId = null,
        Dictionary<string, object>? metadata = null);

    /// <summary>
    /// Mark notification as read
    /// </summary>
    System.Threading.Tasks.Task MarkAsReadAsync(Guid notificationId, Guid userId);

    /// <summary>
    /// Mark all notifications as read for a user
    /// </summary>
    System.Threading.Tasks.Task MarkAllAsReadAsync(Guid userId);

    /// <summary>
    /// Get user's notifications
    /// </summary>
    System.Threading.Tasks.Task<List<Notification>> GetUserNotificationsAsync(Guid userId, bool includeRead = true, int count = 50);

    /// <summary>
    /// Get unread notification count
    /// </summary>
    System.Threading.Tasks.Task<int> GetUnreadCountAsync(Guid userId);

    /// <summary>
    /// Check if user should receive notification based on preferences
    /// </summary>
    System.Threading.Tasks.Task<bool> ShouldSendNotificationAsync(Guid userId, string notificationType);
}
