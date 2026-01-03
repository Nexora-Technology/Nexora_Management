using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

/// <summary>
/// User notification for real-time alerts
/// </summary>
public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid? WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }

    public string Type { get; set; } = string.Empty; // "task_assigned", "comment_mentioned", "status_changed", etc.
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Optional metadata for additional notification info
    public Dictionary<string, object>? Metadata { get; set; }
}
