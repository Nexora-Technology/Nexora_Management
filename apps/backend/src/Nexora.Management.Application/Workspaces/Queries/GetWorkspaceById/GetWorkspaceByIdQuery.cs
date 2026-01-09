using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Workspaces.Queries.GetWorkspaceById;

public record GetWorkspaceByIdQuery(Guid Id) : IRequest<Result<WorkspaceDto>>;

public class GetWorkspaceByIdQueryHandler : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceDto>>
{
    private readonly IAppDbContext _db;

    public GetWorkspaceByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<WorkspaceDto>> Handle(GetWorkspaceByIdQuery request, CancellationToken ct)
    {
        var workspace = await _db.Workspaces
            .Include(w => w.Owner)
            .FirstOrDefaultAsync(w => w.Id == request.Id, ct);

        if (workspace == null)
        {
            return Result<WorkspaceDto>.Failure("Workspace not found");
        }

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
