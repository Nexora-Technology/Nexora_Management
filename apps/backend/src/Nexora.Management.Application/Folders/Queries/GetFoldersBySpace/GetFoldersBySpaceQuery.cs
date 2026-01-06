using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Folders.Queries.GetFoldersBySpace;

public record GetFoldersBySpaceQuery(Guid SpaceId) : IRequest<Result<List<FolderDto>>>;

public class GetFoldersBySpaceQueryHandler : IRequestHandler<GetFoldersBySpaceQuery, Result<List<FolderDto>>>
{
    private readonly IAppDbContext _db;

    public GetFoldersBySpaceQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<FolderDto>>> Handle(GetFoldersBySpaceQuery request, CancellationToken ct)
    {
        var folders = await _db.Folders
            .AsNoTracking()
            .Where(f => f.SpaceId == request.SpaceId)
            .OrderBy(f => f.PositionOrder)
            .Select(f => new FolderDto(
                f.Id,
                f.SpaceId,
                f.Name,
                f.Description,
                f.Color,
                f.Icon,
                f.PositionOrder,
                f.CreatedAt,
                f.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<FolderDto>>.Success(folders);
    }
}
