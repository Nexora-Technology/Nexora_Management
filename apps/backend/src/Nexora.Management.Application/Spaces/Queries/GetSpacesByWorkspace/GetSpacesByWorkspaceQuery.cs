using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Spaces.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Spaces.Queries.GetSpacesByWorkspace;

public record GetSpacesByWorkspaceQuery(Guid WorkspaceId) : IRequest<Result<List<SpaceDto>>>;

public class GetSpacesByWorkspaceQueryHandler : IRequestHandler<GetSpacesByWorkspaceQuery, Result<List<SpaceDto>>>
{
    private readonly IAppDbContext _db;

    public GetSpacesByWorkspaceQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<SpaceDto>>> Handle(GetSpacesByWorkspaceQuery request, CancellationToken ct)
    {
        var spaces = await _db.Spaces
            .AsNoTracking()
            .Where(s => s.WorkspaceId == request.WorkspaceId)
            .OrderBy(s => s.Name)
            .Select(s => new SpaceDto(
                s.Id,
                s.WorkspaceId,
                s.Name,
                s.Description,
                s.Color,
                s.Icon,
                s.IsPrivate,
                s.CreatedAt,
                s.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<SpaceDto>>.Success(spaces);
    }
}
