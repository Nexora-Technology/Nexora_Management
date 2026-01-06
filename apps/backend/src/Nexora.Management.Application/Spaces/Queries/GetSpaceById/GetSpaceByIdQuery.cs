using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Spaces.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Spaces.Queries.GetSpaceById;

public record GetSpaceByIdQuery(Guid Id) : IRequest<Result<SpaceDto>>;

public class GetSpaceByIdQueryHandler : IRequestHandler<GetSpaceByIdQuery, Result<SpaceDto>>
{
    private readonly IAppDbContext _db;

    public GetSpaceByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<SpaceDto>> Handle(GetSpaceByIdQuery request, CancellationToken ct)
    {
        var space = await _db.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (space == null)
        {
            return Result<SpaceDto>.Failure("Space not found");
        }

        var spaceDto = new SpaceDto(
            space.Id,
            space.WorkspaceId,
            space.Name,
            space.Description,
            space.Color,
            space.Icon,
            space.IsPrivate,
            space.CreatedAt,
            space.UpdatedAt
        );

        return Result<SpaceDto>.Success(spaceDto);
    }
}
