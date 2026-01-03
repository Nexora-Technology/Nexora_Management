using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

/// <summary>
/// Tracks user presence and online status per workspace
/// </summary>
public class UserPresence : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }

    public string? ConnectionId { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public bool IsOnline { get; set; } = false;

    // Optional metadata for additional presence info
    public Dictionary<string, object>? Metadata { get; set; }
}
