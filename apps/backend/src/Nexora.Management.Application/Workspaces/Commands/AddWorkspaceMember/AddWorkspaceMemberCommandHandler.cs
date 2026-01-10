using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Workspaces.Commands.AddWorkspaceMember;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Commands.AddWorkspaceMember;

/// <summary>
/// Handler for adding a member to a workspace
/// </summary>
public class AddWorkspaceMemberCommandHandler : IRequestHandler<AddWorkspaceMemberCommand, Result<WorkspaceMemberResponse>>
{
    private readonly AppDbContext _dbContext;

    public AddWorkspaceMemberCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WorkspaceMemberResponse>> Handle(AddWorkspaceMemberCommand command, CancellationToken cancellationToken)
    {
        var workspaceId = command.WorkspaceId;
        var request = command.Request;

        // Validate workspace exists
        var workspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId, cancellationToken);

        if (workspace == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Workspace not found");
        }

        // Validate user exists
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("User not found");
        }

        // Validate role exists
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Role not found");
        }

        // Check if user is already a member
        var existingMember = await _dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == request.UserId, cancellationToken);

        if (existingMember != null)
        {
            return Result<WorkspaceMemberResponse>.Failure("User is already a member of this workspace");
        }

        // Create workspace member
        var workspaceMember = new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = request.UserId,
            RoleId = request.RoleId
        };

        _dbContext.WorkspaceMembers.Add(workspaceMember);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return response with joined data
        var response = new WorkspaceMemberResponse
        {
            Id = workspaceMember.Id,
            WorkspaceId = workspaceMember.WorkspaceId,
            UserId = workspaceMember.UserId,
            UserEmail = user.Email,
            UserName = user.Name,
            RoleId = role.Id,
            RoleName = role.Name,
            JoinedAt = workspaceMember.CreatedAt
        };

        return Result<WorkspaceMemberResponse>.Success(response);
    }
}
