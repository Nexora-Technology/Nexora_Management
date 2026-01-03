using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.API.Hubs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Infrastructure.Persistence;
using System.Collections.Concurrent;
using Task = System.Threading.Tasks.Task;

namespace Nexora.Management.API.Services;

/// <summary>
/// In-memory and database-backed service for tracking user presence
/// </summary>
public class PresenceService : IPresenceService
{
    private readonly IAppDbContext _dbContext;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly ILogger<PresenceService> _logger;

    // In-memory cache for active connections
    private readonly ConcurrentDictionary<string, (Guid UserId, Guid WorkspaceId, DateTime LastSeen)> _activeConnections = new();

    public PresenceService(
        IAppDbContext dbContext,
        IHubContext<PresenceHub> presenceHub,
        ILogger<PresenceService> logger)
    {
        _dbContext = dbContext;
        _presenceHub = presenceHub;
        _logger = logger;
    }

    public async Task TrackConnectionAsync(Guid userId, Guid workspaceId, string connectionId)
    {
        // Update in-memory cache
        _activeConnections.AddOrUpdate(
            connectionId,
            (userId, workspaceId, DateTime.UtcNow),
            (key, existing) => (existing.UserId, existing.WorkspaceId, DateTime.UtcNow)
        );

        // Update or create database record
        var existingPresence = await _dbContext.UserPresences
            .FirstOrDefaultAsync(up => up.UserId == userId && up.WorkspaceId == workspaceId);

        if (existingPresence != null)
        {
            existingPresence.ConnectionId = connectionId;
            existingPresence.LastSeen = DateTime.UtcNow;
            existingPresence.IsOnline = true;
        }
        else
        {
            var newPresence = new UserPresence
            {
                UserId = userId,
                WorkspaceId = workspaceId,
                ConnectionId = connectionId,
                LastSeen = DateTime.UtcNow,
                IsOnline = true
            };
            _dbContext.UserPresences.Add(newPresence);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogDebug("Tracked connection for user {UserId} in workspace {WorkspaceId}", userId, workspaceId);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (_activeConnections.TryRemove(connectionId, out var connectionInfo))
        {
            // Check if user has other active connections
            var hasOtherConnections = _activeConnections.Values
                .Any(c => c.UserId == connectionInfo.UserId && c.WorkspaceId == connectionInfo.WorkspaceId);

            if (!hasOtherConnections)
            {
                // Mark as offline if no other connections
                var presence = await _dbContext.UserPresences
                    .FirstOrDefaultAsync(up => up.UserId == connectionInfo.UserId && up.WorkspaceId == connectionInfo.WorkspaceId);

                if (presence != null)
                {
                    presence.IsOnline = false;
                    presence.LastSeen = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
            }

            _logger.LogDebug("Removed connection {ConnectionId}", connectionId);
        }
    }

    public async Task UpdateLastSeenAsync(Guid userId)
    {
        var presences = await _dbContext.UserPresences
            .Where(up => up.UserId == userId)
            .ToListAsync();

        foreach (var presence in presences)
        {
            presence.LastSeen = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<UserPresence>> GetOnlineUsersAsync(Guid workspaceId)
    {
        return await _dbContext.UserPresences
            .Where(up => up.WorkspaceId == workspaceId && up.IsOnline)
            .Include(up => up.User)
            .ToListAsync();
    }

    public async Task<UserPresence?> GetUserPresenceAsync(Guid userId, Guid workspaceId)
    {
        return await _dbContext.UserPresences
            .Include(up => up.User)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.WorkspaceId == workspaceId);
    }

    public async Task CleanupStaleConnectionsAsync()
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-5);

        // Find stale connections in database
        var stalePresences = await _dbContext.UserPresences
            .Where(up => up.IsOnline && up.LastSeen < cutoffTime)
            .ToListAsync();

        foreach (var presence in stalePresences)
        {
            presence.IsOnline = false;
        }

        await _dbContext.SaveChangesAsync();

        // Clean up in-memory cache
        var staleConnections = _activeConnections
            .Where(kvp => kvp.Value.LastSeen < cutoffTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var connectionId in staleConnections)
        {
            _activeConnections.TryRemove(connectionId, out _);
        }

        if (stalePresences.Count > 0)
        {
            _logger.LogInformation("Cleaned up {Count} stale connections", stalePresences.Count);
        }
    }
}
