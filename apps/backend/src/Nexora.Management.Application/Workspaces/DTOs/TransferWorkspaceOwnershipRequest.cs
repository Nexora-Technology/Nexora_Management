namespace Nexora.Management.Application.Workspaces.DTOs;

/// <summary>
/// Request to transfer workspace ownership to another member
/// </summary>
public record TransferWorkspaceOwnershipRequest
{
    /// <summary>
    /// ID of the user to transfer ownership to (must be an existing member)
    /// </summary>
    public Guid NewOwnerId { get; init; }
}
