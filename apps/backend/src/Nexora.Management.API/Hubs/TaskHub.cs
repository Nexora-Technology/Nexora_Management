using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Nexora.Management.API.Hubs;

/// <summary>
/// Hub for real-time task updates within tasklists
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
    /// Join a tasklist group to receive task updates
    /// </summary>
    public async Task JoinTaskList(Guid taskListId)
    {
        var groupName = GetTaskListGroupName(taskListId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} joined tasklist {TaskListId}", Context.UserIdentifier, taskListId);
    }

    /// <summary>
    /// Leave a tasklist group
    /// </summary>
    public async Task LeaveTaskList(Guid taskListId)
    {
        var groupName = GetTaskListGroupName(taskListId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} left tasklist {TaskListId}", Context.UserIdentifier, taskListId);
    }

    /// <summary>
    /// Broadcast task update to tasklist group (called by server)
    /// </summary>
    public async Task BroadcastTaskUpdate(Guid taskListId, string messageType, object data)
    {
        var groupName = GetTaskListGroupName(taskListId);
        await Clients.Group(groupName).SendAsync(messageType, data);
        _logger.LogDebug("Broadcasted {MessageType} to tasklist {TaskListId}", messageType, taskListId);
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

    private static string GetTaskListGroupName(Guid taskListId) => $"tasklist_{taskListId}";
}
