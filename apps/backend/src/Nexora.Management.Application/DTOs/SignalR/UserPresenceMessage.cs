namespace Nexora.Management.Application.DTOs.SignalR;

/// <summary>
/// Message for user presence tracking (online/offline/viewing)
/// </summary>
public class UserPresenceMessage
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid WorkspaceId { get; set; }
    public string? ConnectionId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
}
