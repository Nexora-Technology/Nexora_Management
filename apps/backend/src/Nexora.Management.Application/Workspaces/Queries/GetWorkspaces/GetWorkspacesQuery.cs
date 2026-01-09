using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Workspaces.Queries.GetWorkspaces;

public record GetWorkspacesQuery(Guid? UserId) : IRequest<Result<List<WorkspaceDto>>>;

public class GetWorkspacesQueryHandler : IRequestHandler<GetWorkspacesQuery, Result<List<WorkspaceDto>>>
{
    private readonly IAppDbContext _db;

    public GetWorkspacesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<WorkspaceDto>>> Handle(GetWorkspacesQuery request, CancellationToken ct)
    {
        var query = _db.Workspaces.Include(w => w.Owner).AsQueryable();

        // If UserId provided, filter by owner
        if (request.UserId.HasValue)
        {
            query = query.Where(w => w.OwnerId == request.UserId.Value);
        }

        var workspaces = await query
            .OrderByDescending(w => w.UpdatedAt)
            .ToListAsync(ct);

        var workspaceDtos = workspaces.Select(w => new WorkspaceDto(
            w.Id,
            w.Name,
            w.OwnerId,
            w.Owner?.Name ?? string.Empty,
            w.SettingsJsonb,
            w.CreatedAt,
            w.UpdatedAt
        )).ToList();

        return Result<List<WorkspaceDto>>.Success(workspaceDtos);
    }
}
