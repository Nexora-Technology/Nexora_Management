using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Workspaces.Commands.TransferWorkspaceOwnership;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Persistence;

namespace Nexora.Management.Application.Workspaces.Commands.TransferWorkspaceOwnership;

/// <summary>
/// Handler for transferring workspace ownership to another member
/// </summary>
public class TransferWorkspaceOwnershipCommandHandler : IRequestHandler<TransferWorkspaceOwnershipCommand, Result<WorkspaceMemberResponse>>
{
    private readonly AppDbContext _dbContext;

    public TransferWorkspaceOwnershipCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WorkspaceMemberResponse>> Handle(TransferWorkspaceOwnershipCommand command, CancellationToken cancellationToken)
    {
        var workspaceId = command.WorkspaceId;
        var currentOwnerId = command.CurrentOwnerId;
        var request = command.Request;

        // Validate workspace exists
        var workspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId, cancellationToken);

        if (workspace == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Workspace not found");
        }

        // Validate caller is the current owner
        if (workspace.OwnerId != currentOwnerId)
        {
            return Result<WorkspaceMemberResponse>.Failure("Only the workspace owner can transfer ownership");
        }

        // Cannot transfer to yourself
        if (currentOwnerId == request.NewOwnerId)
        {
            return Result<WorkspaceMemberResponse>.Failure("Cannot transfer ownership to yourself");
        }

        // Validate new owner is a member of the workspace
        var newOwnerMember = await _dbContext.WorkspaceMembers
            .Include(wm => wm.User)
            .Include(wm => wm.Role)
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == request.NewOwnerId, cancellationToken);

        if (newOwnerMember == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("New owner is not a member of this workspace");
        }

        // Get or create Admin role for the current owner
        var adminRole = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin", cancellationToken);

        if (adminRole == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Admin role not found in system");
        }

        // Get or create Owner role for the new owner
        var ownerRole = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "Owner", cancellationToken);

        if (ownerRole == null)
        {
            return Result<WorkspaceMemberResponse>.Failure("Owner role not found in system");
        }

        // Transfer ownership
        workspace.OwnerId = request.NewOwnerId;

        // Update new owner's role to Owner
        newOwnerMember.RoleId = ownerRole.Id;

        // Find current owner's member record (if exists as a member)
        var currentOwnerMember = await _dbContext.WorkspaceMembers
            .Include(wm => wm.User)
            .Include(wm => wm.Role)
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == currentOwnerId, cancellationToken);

        // Demote current owner to Admin role
        if (currentOwnerMember != null)
        {
            currentOwnerMember.RoleId = adminRole.Id;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return response with the new owner's details
        var response = new WorkspaceMemberResponse
        {
            Id = newOwnerMember.Id,
            WorkspaceId = newOwnerMember.WorkspaceId,
            UserId = newOwnerMember.UserId,
            UserEmail = newOwnerMember.User?.Email ?? string.Empty,
            UserName = newOwnerMember.User?.Name ?? string.Empty,
            RoleId = ownerRole.Id,
            RoleName = ownerRole.Name,
            JoinedAt = newOwnerMember.CreatedAt
        };

        return Result<WorkspaceMemberResponse>.Success(response);
    }
}
