using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.API.Hubs;
using Nexora.Management.Application.DTOs.SignalR;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.API.Services;

/// <summary>
/// Service for creating and delivering notifications
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IAppDbContext _dbContext;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IAppDbContext dbContext,
        IHubContext<NotificationHub> notificationHub,
        ILogger<NotificationService> logger)
    {
        _dbContext = dbContext;
        _notificationHub = notificationHub;
        _logger = logger;
    }

    public async Task<Notification> CreateNotificationAsync(
        Guid userId,
        string type,
        string title,
        string? message = null,
        string? actionUrl = null,
        Guid? workspaceId = null,
        Dictionary<string, object>? metadata = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkspaceId = workspaceId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            Metadata = metadata,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        // Send real-time notification
        var notificationMessage = new NotificationMessage
        {
            NotificationId = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            ActionUrl = notification.ActionUrl,
            CreatedAt = notification.CreatedAt
        };

        await SendNotificationAsync(userId, notificationMessage);

        return notification;
    }

    private async Task SendNotificationAsync(Guid userId, NotificationMessage notification)
    {
        var groupName = $"user_{userId}";
        await _notificationHub.Clients.Group(groupName).SendAsync("NotificationReceived", notification);
        _logger.LogDebug("Sent notification {NotificationId} to user {UserId}", notification.NotificationId, userId);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, bool includeRead = true, int count = 50)
    {
        var query = _dbContext.Notifications
            .Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _dbContext.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<bool> ShouldSendNotificationAsync(Guid userId, string notificationType)
    {
        // Get user preferences
        var preferences = await _dbContext.NotificationPreferences
            .FirstOrDefaultAsync(np => np.UserId == userId);

        if (preferences == null)
        {
            // Default to true if no preferences set
            return true;
        }

        // Check if in-app notifications are enabled
        if (!preferences.InAppEnabled)
        {
            return false;
        }

        // Check notification type preference
        return notificationType switch
        {
            "task_assigned" => preferences.TaskAssignedEnabled,
            "comment_mentioned" => preferences.CommentMentionedEnabled,
            "status_changed" => preferences.StatusChangedEnabled,
            "due_date_reminder" => preferences.DueDateReminderEnabled,
            "project_invitation" => preferences.ProjectInvitationEnabled,
            _ => true
        };
    }
}
