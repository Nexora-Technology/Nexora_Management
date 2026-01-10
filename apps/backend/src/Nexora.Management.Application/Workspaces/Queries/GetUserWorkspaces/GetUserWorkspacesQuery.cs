using MediatR;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;

namespace Nexora.Management.Application.Workspaces.Queries.GetUserWorkspaces;

/// <summary>
/// Query to get all workspaces for a specific user
/// </summary>
public record GetUserWorkspacesQuery(Guid UserId) : IRequest<Result<List<UserWorkspaceResponse>>>;
