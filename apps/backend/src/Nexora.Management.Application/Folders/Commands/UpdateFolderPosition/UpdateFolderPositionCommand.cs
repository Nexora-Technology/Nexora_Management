using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Folders.Commands.UpdateFolderPosition;

public record UpdateFolderPositionCommand(Guid Id, int PositionOrder) : IRequest<Result<FolderDto>>;

public class UpdateFolderPositionCommandHandler : IRequestHandler<UpdateFolderPositionCommand, Result<FolderDto>>
{
    private readonly IAppDbContext _db;

    public UpdateFolderPositionCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<FolderDto>> Handle(UpdateFolderPositionCommand request, CancellationToken ct)
    {
        var folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == request.Id, ct);
        if (folder == null)
        {
            return Result<FolderDto>.Failure("Folder not found");
        }

        folder.PositionOrder = request.PositionOrder;
        await _db.SaveChangesAsync(ct);

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
