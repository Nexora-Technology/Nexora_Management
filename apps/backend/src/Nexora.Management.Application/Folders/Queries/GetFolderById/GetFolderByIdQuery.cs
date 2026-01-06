using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Folders.Queries.GetFolderById;

public record GetFolderByIdQuery(Guid Id) : IRequest<Result<FolderDto>>;

public class GetFolderByIdQueryHandler : IRequestHandler<GetFolderByIdQuery, Result<FolderDto>>
{
    private readonly IAppDbContext _db;

    public GetFolderByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<FolderDto>> Handle(GetFolderByIdQuery request, CancellationToken ct)
    {
        var folder = await _db.Folders
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.Id, ct);

        if (folder == null)
        {
            return Result<FolderDto>.Failure("Folder not found");
        }

        var folderDto = new FolderDto(
            folder.Id,
            folder.SpaceId,
            folder.Name,
            folder.Description,
            folder.Color,
            folder.Icon,
            folder.PositionOrder,
            folder.CreatedAt,
            folder.UpdatedAt
        );

        return Result<FolderDto>.Success(folderDto);
    }
}
