using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Spaces.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Spaces.Commands.CreateSpace;

public record CreateSpaceCommand(
    Guid WorkspaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    bool IsPrivate = false
) : IRequest<Result<SpaceDto>>;

public class CreateSpaceCommandHandler : IRequestHandler<CreateSpaceCommand, Result<SpaceDto>>
{
    private readonly IAppDbContext _db;

    public CreateSpaceCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<SpaceDto>> Handle(CreateSpaceCommand request, CancellationToken ct)
    {
        // Validate workspace exists
        var workspace = await _db.Workspaces.FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, ct);
        if (workspace == null)
        {
            return Result<SpaceDto>.Failure("Workspace not found");
        }

        var space = new Space
        {
            WorkspaceId = request.WorkspaceId,
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            IsPrivate = request.IsPrivate,
            SettingsJsonb = new Dictionary<string, object>()
        };

        _db.Spaces.Add(space);
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
