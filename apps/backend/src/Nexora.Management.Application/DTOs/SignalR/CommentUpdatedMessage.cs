namespace Nexora.Management.Application.DTOs.SignalR;

/// <summary>
/// Message broadcasted when a comment is added, updated, or deleted
/// </summary>
public class CommentUpdatedMessage
{
    public Guid CommentId { get; set; }
    public Guid TaskId { get; set; }
    public string Type { get; set; } = string.Empty; // "added", "updated", "deleted"
    public Guid UpdatedBy { get; set; }
    public DateTime Timestamp { get; set; }
    public object? Data { get; set; }
}
