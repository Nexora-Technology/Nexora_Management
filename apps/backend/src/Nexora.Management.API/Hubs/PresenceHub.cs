using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.API.Hubs;

/// <summary>
/// Hub for tracking user presence and online status
/// </summary>
[Authorize]
public class PresenceHub : Hub
{
    private readonly ILogger<PresenceHub> _logger;
    private readonly IPresenceService _presenceService;

    public PresenceHub(ILogger<PresenceHub> logger, IPresenceService presenceService)
    {
        _logger = logger;
        _presenceService = presenceService;
    }

    /// <summary>
    /// Join a workspace to broadcast presence
    /// </summary>
    public async Task JoinWorkspace(Guid workspaceId)
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            _logger.LogWarning("Invalid user ID in context: {UserIdentifier}", Context.UserIdentifier);
            return;
        }

        var groupName = GetWorkspaceGroupName(workspaceId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Track connection in presence service
        await _presenceService.TrackConnectionAsync(userId, workspaceId, Context.ConnectionId);

        // Broadcast user joined to workspace
        await Clients.Group(groupName).SendAsync("UserJoined", new
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            Timestamp = DateTime.UtcNow
        });

        _logger.LogInformation("User {UserId} joined workspace {WorkspaceId}", userId, workspaceId);
    }

    /// <summary>
    /// Leave a workspace
    /// </summary>
    public async Task LeaveWorkspace(Guid workspaceId)
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            return;
        }

        var groupName = GetWorkspaceGroupName(workspaceId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        // Remove connection from presence service
        await _presenceService.RemoveConnectionAsync(Context.ConnectionId);

        // Broadcast user left to workspace
        await Clients.Group(groupName).SendAsync("UserLeft", new
        {
            UserId = userId,
            ConnectionId = Context.ConnectionId,
            Timestamp = DateTime.UtcNow
        });

        _logger.LogInformation("User {UserId} left workspace {WorkspaceId}", userId, workspaceId);
    }

    /// <summary>
    /// Update last seen timestamp
    /// </summary>
    public async Task UpdateLastSeen()
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            return;
        }

        await _presenceService.UpdateLastSeenAsync(userId);
    }

    /// <summary>
    /// Start typing indicator
    /// </summary>
    public async Task StartTyping(Guid taskId)
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            return;
        }

        await Clients.Group($"task_{taskId}").SendAsync("UserTyping", new
        {
            UserId = userId,
            TaskId = taskId,
            IsTyping = true,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Stop typing indicator
    /// </summary>
    public async Task StopTyping(Guid taskId)
    {
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            return;
        }

        await Clients.Group($"task_{taskId}").SendAsync("UserTyping", new
        {
            UserId = userId,
            TaskId = taskId,
            IsTyping = false,
            Timestamp = DateTime.UtcNow
        });
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("PresenceHub connected: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Remove connection from presence service
        await _presenceService.RemoveConnectionAsync(Context.ConnectionId);

        if (exception != null)
        {
            _logger.LogWarning(exception, "PresenceHub disconnected: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("PresenceHub disconnected: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static string GetWorkspaceGroupName(Guid workspaceId) => $"workspace_{workspaceId}";
}
