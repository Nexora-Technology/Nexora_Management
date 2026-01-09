using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand(
    string Name,
    Guid OwnerId,
    Dictionary<string, object>? SettingsJsonb
) : IRequest<Result<WorkspaceDto>>;

public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, Result<WorkspaceDto>>
{
    private readonly IAppDbContext _db;

    public CreateWorkspaceCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<WorkspaceDto>> Handle(CreateWorkspaceCommand request, CancellationToken ct)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            OwnerId = request.OwnerId,
            SettingsJsonb = request.SettingsJsonb ?? new Dictionary<string, object>()
        };

        _db.Workspaces.Add(workspace);
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
