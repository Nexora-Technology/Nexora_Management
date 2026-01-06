using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Folders.Commands.UpdateFolder;

public record UpdateFolderCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    string? Icon
) : IRequest<Result<FolderDto>>;

public class UpdateFolderCommandHandler : IRequestHandler<UpdateFolderCommand, Result<FolderDto>>
{
    private readonly IAppDbContext _db;

    public UpdateFolderCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<FolderDto>> Handle(UpdateFolderCommand request, CancellationToken ct)
    {
        var folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == request.Id, ct);
        if (folder == null)
        {
            return Result<FolderDto>.Failure("Folder not found");
        }

        folder.Name = request.Name;
        folder.Description = request.Description;
        folder.Color = request.Color;
        folder.Icon = request.Icon;

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
