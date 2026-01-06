using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Spaces.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Spaces.Commands.UpdateSpace;

public record UpdateSpaceCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    bool IsPrivate
) : IRequest<Result<SpaceDto>>;

public class UpdateSpaceCommandHandler : IRequestHandler<UpdateSpaceCommand, Result<SpaceDto>>
{
    private readonly IAppDbContext _db;

    public UpdateSpaceCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<SpaceDto>> Handle(UpdateSpaceCommand request, CancellationToken ct)
    {
        var space = await _db.Spaces.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (space == null)
        {
            return Result<SpaceDto>.Failure("Space not found");
        }

        space.Name = request.Name;
        space.Description = request.Description;
        space.Color = request.Color;
        space.Icon = request.Icon;
        space.IsPrivate = request.IsPrivate;

        await _db.SaveChangesAsync(ct);

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
