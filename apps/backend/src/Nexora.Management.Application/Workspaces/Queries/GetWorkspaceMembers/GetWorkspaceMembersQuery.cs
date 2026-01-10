using MediatR;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;

namespace Nexora.Management.Application.Workspaces.Queries.GetWorkspaceMembers;

/// <summary>
/// Query to get all members of a workspace
/// </summary>
public record GetWorkspaceMembersQuery(Guid WorkspaceId) : IRequest<Result<List<WorkspaceMemberResponse>>>;
