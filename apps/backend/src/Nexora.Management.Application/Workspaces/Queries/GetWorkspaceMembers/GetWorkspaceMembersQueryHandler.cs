using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Queries.GetWorkspaceMembers;

/// <summary>
/// Handler for getting all members of a workspace
/// </summary>
public class GetWorkspaceMembersQueryHandler : IRequestHandler<GetWorkspaceMembersQuery, Result<List<WorkspaceMemberResponse>>>
{
    private readonly AppDbContext _dbContext;

    public GetWorkspaceMembersQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<WorkspaceMemberResponse>>> Handle(GetWorkspaceMembersQuery query, CancellationToken cancellationToken)
    {
        // Validate workspace exists
        var workspaceExists = await _dbContext.Workspaces
            .AnyAsync(w => w.Id == query.WorkspaceId, cancellationToken);

        if (!workspaceExists)
        {
            return Result<List<WorkspaceMemberResponse>>.Failure("Workspace not found");
        }

        // Get all members with their user and role details
        var members = await _dbContext.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == query.WorkspaceId)
            .Include(wm => wm.User)
            .Include(wm => wm.Role)
            .OrderBy(wm => wm.Role.Name) // Order by role name (Owner, Admin, Member, Guest)
            .ThenBy(wm => wm.User != null ? wm.User.Name : string.Empty)
            .Select(wm => new WorkspaceMemberResponse
            {
                Id = wm.Id,
                WorkspaceId = wm.WorkspaceId,
                UserId = wm.UserId,
                UserEmail = wm.User != null ? wm.User.Email : string.Empty,
                UserName = wm.User != null ? wm.User.Name : string.Empty,
                RoleId = wm.RoleId,
                RoleName = wm.Role != null ? wm.Role.Name : string.Empty,
                JoinedAt = wm.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<WorkspaceMemberResponse>>.Success(members);
    }
}
