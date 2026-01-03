namespace Nexora.Management.Infrastructure.Interfaces;

using Nexora.Management.Domain.Entities;

/// <summary>
/// Service for tracking user presence and connections
/// </summary>
public interface IPresenceService
{
    /// <summary>
    /// Track a user connection in a workspace
    /// </summary>
    System.Threading.Tasks.Task TrackConnectionAsync(Guid userId, Guid workspaceId, string connectionId);

    /// <summary>
    /// Remove a connection when user disconnects
    /// </summary>
    System.Threading.Tasks.Task RemoveConnectionAsync(string connectionId);

    /// <summary>
    /// Update user's last seen timestamp
    /// </summary>
    System.Threading.Tasks.Task UpdateLastSeenAsync(Guid userId);

    /// <summary>
    /// Get all online users in a workspace
    /// </summary>
    System.Threading.Tasks.Task<List<UserPresence>> GetOnlineUsersAsync(Guid workspaceId);

    /// <summary>
    /// Get user presence in a workspace
    /// </summary>
    System.Threading.Tasks.Task<UserPresence?> GetUserPresenceAsync(Guid userId, Guid workspaceId);

    /// <summary>
    /// Clean up stale connections
    /// </summary>
    System.Threading.Tasks.Task CleanupStaleConnectionsAsync();
}
