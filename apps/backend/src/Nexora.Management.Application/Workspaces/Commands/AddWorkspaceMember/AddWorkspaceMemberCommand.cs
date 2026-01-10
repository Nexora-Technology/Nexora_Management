using MediatR;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Common;

namespace Nexora.Management.Application.Workspaces.Commands.AddWorkspaceMember;

/// <summary>
/// Command to add a member to a workspace
/// </summary>
public record AddWorkspaceMemberCommand(
    Guid WorkspaceId,
    AddWorkspaceMemberRequest Request
) : IRequest<Result<WorkspaceMemberResponse>>;
