namespace Nexora.Management.Application.DTOs.SignalR;

/// <summary>
/// Real-time notification message
/// </summary>
public class NotificationMessage
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty; // "task_assigned", "comment_mentioned", "status_changed", etc.
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
