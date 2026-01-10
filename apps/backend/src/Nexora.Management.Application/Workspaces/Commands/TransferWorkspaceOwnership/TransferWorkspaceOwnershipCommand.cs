using MediatR;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Common;

namespace Nexora.Management.Application.Workspaces.Commands.TransferWorkspaceOwnership;

/// <summary>
/// Command to transfer workspace ownership to another member
/// </summary>
public record TransferWorkspaceOwnershipCommand(
    Guid WorkspaceId,
    Guid CurrentOwnerId,
    TransferWorkspaceOwnershipRequest Request
) : IRequest<Result<WorkspaceMemberResponse>>;
