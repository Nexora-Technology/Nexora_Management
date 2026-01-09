using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Workspaces.Commands.UpdateWorkspace;

public record UpdateWorkspaceCommand(
    Guid Id,
    string Name,
    Dictionary<string, object>? SettingsJsonb
) : IRequest<Result<WorkspaceDto>>;

public class UpdateWorkspaceCommandHandler : IRequestHandler<UpdateWorkspaceCommand, Result<WorkspaceDto>>
{
    private readonly IAppDbContext _db;

    public UpdateWorkspaceCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<WorkspaceDto>> Handle(UpdateWorkspaceCommand request, CancellationToken ct)
    {
        var workspace = await _db.Workspaces
            .Include(w => w.Owner)
            .FirstOrDefaultAsync(w => w.Id == request.Id, ct);

        if (workspace == null)
        {
            return Result<WorkspaceDto>.Failure("Workspace not found");
        }

        workspace.Name = request.Name;
        if (request.SettingsJsonb != null)
        {
            workspace.SettingsJsonb = request.SettingsJsonb;
        }

        await _db.SaveChangesAsync(ct);

        var workspaceDto = new WorkspaceDto(
            workspace.Id,
            workspace.Name,
            workspace.OwnerId,
            workspace.Owner?.Name ?? string.Empty,
            workspace.SettingsJsonb,
            workspace.CreatedAt,
            workspace.UpdatedAt
        );

        return Result<WorkspaceDto>.Success(workspaceDto);
    }
}
