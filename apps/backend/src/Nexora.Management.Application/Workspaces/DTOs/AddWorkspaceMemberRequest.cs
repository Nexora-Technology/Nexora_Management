namespace Nexora.Management.Application.Workspaces.DTOs;

/// <summary>
/// Request to add a member to a workspace
/// </summary>
public record AddWorkspaceMemberRequest
{
    /// <summary>
    /// ID of the user to add as member
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// ID of the role to assign to the member
    /// </summary>
    public Guid RoleId { get; init; }
}
