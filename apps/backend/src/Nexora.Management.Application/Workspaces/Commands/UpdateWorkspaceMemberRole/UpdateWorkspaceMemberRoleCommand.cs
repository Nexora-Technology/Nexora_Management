using MediatR;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Common;

namespace Nexora.Management.Application.Workspaces.Commands.UpdateWorkspaceMemberRole;

/// <summary>
/// Command to update a workspace member's role
/// </summary>
public record UpdateWorkspaceMemberRoleCommand(
    Guid WorkspaceId,
    Guid UserId,
    UpdateWorkspaceMemberRoleRequest Request
) : IRequest<Result<WorkspaceMemberResponse>>;
