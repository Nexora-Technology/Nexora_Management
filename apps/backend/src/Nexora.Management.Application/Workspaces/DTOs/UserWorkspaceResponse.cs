namespace Nexora.Management.Application.Workspaces.DTOs;

/// <summary>
/// Response representing a workspace that a user belongs to
/// </summary>
public record UserWorkspaceResponse
{
    /// <summary>
    /// Unique identifier for the workspace
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Workspace name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Whether the user is the owner of this workspace
    /// </summary>
    public bool IsOwner { get; init; }

    /// <summary>
    /// Total number of members in the workspace
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// When the workspace was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// When the workspace was last updated
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}
