namespace Nexora.Management.Application.DTOs.SignalR;

/// <summary>
/// Message broadcasted when a task is created, updated, or deleted
/// </summary>
public class TaskUpdatedMessage
{
    public Guid TaskId { get; set; }
    public Guid TaskListId { get; set; }
    public string Type { get; set; } = string.Empty; // "created", "updated", "deleted"
    public Guid UpdatedBy { get; set; }
    public DateTime Timestamp { get; set; }
    public object? Data { get; set; }
}
