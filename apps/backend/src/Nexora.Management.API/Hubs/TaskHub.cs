using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Nexora.Management.API.Hubs;

/// <summary>
/// Hub for real-time task updates within projects
/// </summary>
[Authorize]
public class TaskHub : Hub
{
    private readonly ILogger<TaskHub> _logger;

    public TaskHub(ILogger<TaskHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Join a project group to receive task updates
    /// </summary>
    public async Task JoinProject(Guid projectId)
    {
        var groupName = GetProjectGroupName(projectId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} joined project {ProjectId}", Context.UserIdentifier, projectId);
    }

    /// <summary>
    /// Leave a project group
    /// </summary>
    public async Task LeaveProject(Guid projectId)
    {
        var groupName = GetProjectGroupName(projectId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} left project {ProjectId}", Context.UserIdentifier, projectId);
    }

    /// <summary>
    /// Broadcast task update to project group (called by server)
    /// </summary>
    public async Task BroadcastTaskUpdate(Guid projectId, string messageType, object data)
    {
        var groupName = GetProjectGroupName(projectId);
        await Clients.Group(groupName).SendAsync(messageType, data);
        _logger.LogDebug("Broadcasted {MessageType} to project {ProjectId}", messageType, projectId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("TaskHub connected: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception, "TaskHub disconnected: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.UserIdentifier);
        }
        else
        {
            _logger.LogInformation("TaskHub disconnected: {ConnectionId}, User: {UserId}", Context.ConnectionId, Context.UserIdentifier);
        }
        await base.OnDisconnectedAsync(exception);
    }

    private static string GetProjectGroupName(Guid projectId) => $"project_{projectId}";
}
