using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.API.Hubs;

/// <summary>
/// Hub for real-time notifications
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
    private readonly INotificationService _notificationService;

    public NotificationHub(ILogger<NotificationHub> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Join user's personal notification group
    /// </summary>
    public async Task JoinUserNotifications()
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            _logger.LogWarning("Invalid user ID in context: {UserIdentifier}", Context.UserIdentifier);
            return;
        }

        var groupName = GetUserGroupName(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        _logger.LogInformation("User {UserId} joined notification group", userId);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    public async Task MarkNotificationRead(Guid notificationId)
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            return;
        }

        await _notificationService.MarkAsReadAsync(notificationId, userId);

        _logger.LogInformation("User {UserId} marked notification {NotificationId} as read", userId, notificationId);
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    public async Task MarkAllNotificationsRead()
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            return;
        }

        await _notificationService.MarkAllAsReadAsync(userId);

        _logger.LogInformation("User {UserId} marked all notifications as read", userId);
    }

    public override async Task OnConnectedAsync()
    {
        if (Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            // Automatically join user's notification group on connect
            await JoinUserNotifications();
        }

        _logger.LogInformation("NotificationHub connected: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception, "NotificationHub disconnected: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("NotificationHub disconnected: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static string GetUserGroupName(Guid userId) => $"user_{userId}";
}
