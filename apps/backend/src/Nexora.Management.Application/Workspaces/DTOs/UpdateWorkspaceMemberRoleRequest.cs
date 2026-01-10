namespace Nexora.Management.Application.Workspaces.DTOs;

/// <summary>
/// Request to update a workspace member's role
/// </summary>
public record UpdateWorkspaceMemberRoleRequest
{
    /// <summary>
    /// New role ID to assign to the member
    /// </summary>
    public Guid RoleId { get; init; }
}
