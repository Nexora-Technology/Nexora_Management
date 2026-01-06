using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Folders.Commands.CreateFolder;

public record CreateFolderCommand(
    Guid SpaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon
) : IRequest<Result<FolderDto>>;

public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, Result<FolderDto>>
{
    private readonly IAppDbContext _db;

    public CreateFolderCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<FolderDto>> Handle(CreateFolderCommand request, CancellationToken ct)
    {
        // Validate space exists
        var space = await _db.Spaces.FirstOrDefaultAsync(s => s.Id == request.SpaceId, ct);
        if (space == null)
        {
            return Result<FolderDto>.Failure("Space not found");
        }

        // Get max position for ordering
        var maxPosition = await _db.Folders
            .Where(f => f.SpaceId == request.SpaceId)
            .MaxAsync(f => (int?)f.PositionOrder) ?? 0;

        var folder = new Folder
        {
            SpaceId = request.SpaceId,
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            PositionOrder = maxPosition + 1,
            SettingsJsonb = new Dictionary<string, object>()
        };

        _db.Folders.Add(folder);
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
