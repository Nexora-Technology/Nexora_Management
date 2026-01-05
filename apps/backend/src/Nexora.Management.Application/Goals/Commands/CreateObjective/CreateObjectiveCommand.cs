using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.CreateObjective;

public record CreateObjectiveCommand(
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string Title,
    string? Description,
    Guid? OwnerId,
    int Weight
) : IRequest<Result<ObjectiveDto>>;

public class CreateObjectiveCommandHandler : IRequestHandler<CreateObjectiveCommand, Result<ObjectiveDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreateObjectiveCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<ObjectiveDto>> Handle(CreateObjectiveCommand request, CancellationToken ct)
    {
        // Validate workspace exists and user has access
        var workspace = await _db.Workspaces.FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, ct);
        if (workspace == null)
        {
            return Result<ObjectiveDto>.Failure("Workspace not found");
        }

        // Validate period if provided
        if (request.PeriodId.HasValue)
        {
            var period = await _db.GoalPeriods.FirstOrDefaultAsync(p => p.Id == request.PeriodId.Value && p.WorkspaceId == request.WorkspaceId, ct);
            if (period == null)
            {
                return Result<ObjectiveDto>.Failure("Goal period not found");
            }
        }

        // Validate parent objective if provided (max 3 levels: root -> child -> grandchild)
        if (request.ParentObjectiveId.HasValue)
        {
            var parent = await _db.Objectives
                .Include(o => o.ParentObjective)
                .FirstOrDefaultAsync(o => o.Id == request.ParentObjectiveId.Value && o.WorkspaceId == request.WorkspaceId, ct);

            if (parent == null)
            {
                return Result<ObjectiveDto>.Failure("Parent objective not found");
            }

            // Check hierarchy depth (3 levels max)
            if (parent.ParentObjectiveId != null)
            {
                return Result<ObjectiveDto>.Failure("Cannot create objective deeper than 3 levels");
            }
        }

        // Get max position for ordering
        var maxPosition = await _db.Objectives
            .Where(o => o.WorkspaceId == request.WorkspaceId && o.ParentObjectiveId == request.ParentObjectiveId)
            .MaxAsync(o => (int?)o.PositionOrder) ?? 0;

        var objective = new Objective
        {
            WorkspaceId = request.WorkspaceId,
            PeriodId = request.PeriodId,
            ParentObjectiveId = request.ParentObjectiveId,
            Title = request.Title,
            Description = request.Description,
            OwnerId = request.OwnerId,
            Weight = request.Weight,
            Status = "on-track",
            Progress = 0,
            PositionOrder = maxPosition + 1
        };

        _db.Objectives.Add(objective);
        await _db.SaveChangesAsync(ct);

        var objectiveDto = new ObjectiveDto(
            objective.Id,
            objective.WorkspaceId,
            objective.PeriodId,
            objective.ParentObjectiveId,
            objective.Title,
            objective.Description,
            objective.OwnerId,
            objective.Weight,
            objective.Status,
            objective.Progress,
            objective.PositionOrder,
            objective.CreatedAt,
            objective.UpdatedAt
        );

        return Result<ObjectiveDto>.Success(objectiveDto);
    }
}
