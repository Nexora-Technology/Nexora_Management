namespace Nexora.Management.Application.Workspaces.DTOs;

/// <summary>
/// Response representing a workspace member with their role
/// </summary>
public record WorkspaceMemberResponse
{
    /// <summary>
    /// Unique identifier for the workspace member relationship
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Workspace ID
    /// </summary>
    public Guid WorkspaceId { get; init; }

    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// User's email address
    /// </summary>
    public string UserEmail { get; init; } = string.Empty;

    /// <summary>
    /// User's display name
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Role ID assigned to this member
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// Role name (Owner, Admin, Member, Guest)
    /// </summary>
    public string RoleName { get; init; } = string.Empty;

    /// <summary>
    /// When the user joined the workspace
    /// </summary>
    public DateTime JoinedAt { get; init; }
}
