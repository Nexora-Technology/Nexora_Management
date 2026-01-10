using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Workspaces.Commands.RemoveWorkspaceMember;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Commands.RemoveWorkspaceMember;

/// <summary>
/// Handler for removing a member from a workspace
/// </summary>
public class RemoveWorkspaceMemberCommandHandler : IRequestHandler<RemoveWorkspaceMemberCommand, Result>
{
    private readonly AppDbContext _dbContext;

    public RemoveWorkspaceMemberCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RemoveWorkspaceMemberCommand command, CancellationToken cancellationToken)
    {
        var workspaceId = command.WorkspaceId;
        var userId = command.UserId;

        // Get workspace to validate ownership
        var workspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId, cancellationToken);

        if (workspace == null)
        {
            return Result.Failure("Workspace not found");
        }

        // Cannot remove the owner
        if (workspace.OwnerId == userId)
        {
            return Result.Failure("Cannot remove workspace owner. Transfer ownership first.");
        }

        // Find the workspace member
        var member = await _dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId, cancellationToken);

        if (member == null)
        {
            return Result.Failure("User is not a member of this workspace");
        }

        _dbContext.WorkspaceMembers.Remove(member);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
