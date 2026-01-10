using MediatR;
using Nexora.Management.Application.Common;

namespace Nexora.Management.Application.Workspaces.Commands.RemoveWorkspaceMember;

/// <summary>
/// Command to remove a member from a workspace
/// </summary>
public record RemoveWorkspaceMemberCommand(
    Guid WorkspaceId,
    Guid UserId
) : IRequest<Result>;
