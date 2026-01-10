using MediatR;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;

namespace Nexora.Management.Application.Workspaces.Queries.GetWorkspaceMemberById;

/// <summary>
/// Query to get a specific workspace member by user ID
/// </summary>
public record GetWorkspaceMemberByIdQuery(
    Guid WorkspaceId,
    Guid UserId
) : IRequest<Result<WorkspaceMemberResponse>>;
