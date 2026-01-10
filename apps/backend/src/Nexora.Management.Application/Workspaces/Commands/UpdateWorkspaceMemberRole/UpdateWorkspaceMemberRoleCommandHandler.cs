using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Workspaces.Commands.UpdateWorkspaceMemberRole;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Commands.UpdateWorkspaceMemberRole;

/// <summary>
/// Handler for updating a workspace member's role
/// </summary>
public class UpdateWorkspaceMemberRoleCommandHandler : IRequestHandler<UpdateWorkspaceMemberRoleCommand, Result<WorkspaceMemberResponse>>
{
    private readonly AppDbContext _dbContext;

    public UpdateWorkspaceMemberRoleCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WorkspaceMemberResponse>> Handle(UpdateWorkspaceMemberRoleCommand command, CancellationToken cancellationToken)
    {
        var workspaceId = command.WorkspaceId;
        var userId = command.UserId;
        var request = command.Request;

        // Validate workspace exists
        var workspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId, cancellationToken);

        if (workspace == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Workspace not found");
        }

        // Cannot change the owner's role
        if (workspace.OwnerId == userId)
        {
            return Result<WorkspaceMemberResponse>.Failure("Cannot change workspace owner's role");
        }

        // Validate new role exists
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Role not found");
        }

        // Find the workspace member
        var member = await _dbContext.WorkspaceMembers
            .Include(wm => wm.User)
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId, cancellationToken);

        if (member == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("User is not a member of this workspace");
        }

        // Update role
        member.RoleId = request.RoleId;
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return response
        var response = new WorkspaceMemberResponse
        {
            Id = member.Id,
            WorkspaceId = member.WorkspaceId,
            UserId = member.UserId,
            UserEmail = member.User?.Email ?? string.Empty,
            UserName = member.User?.Name ?? string.Empty,
            RoleId = role.Id,
            RoleName = role.Name,
            JoinedAt = member.CreatedAt
        };

        return Result<WorkspaceMemberResponse>.Success(response);
    }
}
