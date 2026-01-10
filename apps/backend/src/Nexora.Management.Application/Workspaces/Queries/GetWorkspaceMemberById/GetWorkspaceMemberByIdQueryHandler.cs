using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Queries.GetWorkspaceMemberById;

/// <summary>
/// Handler for getting a specific workspace member
/// </summary>
public class GetWorkspaceMemberByIdQueryHandler : IRequestHandler<GetWorkspaceMemberByIdQuery, Result<WorkspaceMemberResponse>>
{
    private readonly AppDbContext _dbContext;

    public GetWorkspaceMemberByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WorkspaceMemberResponse>> Handle(GetWorkspaceMemberByIdQuery query, CancellationToken cancellationToken)
    {
        // Validate workspace exists
        var workspaceExists = await _dbContext.Workspaces
            .AnyAsync(w => w.Id == query.WorkspaceId, cancellationToken);

        if (!workspaceExists)
        {
            return Result<WorkspaceMemberResponse>.Failure("Workspace not found");
        }

        // Find the specific member
        var member = await _dbContext.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == query.WorkspaceId && wm.UserId == query.UserId)
            .Include(wm => wm.User)
            .Include(wm => wm.Role)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (member == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("User is not a member of this workspace");
        }

        return Result<WorkspaceMemberResponse>.Success(member);
    }
}
