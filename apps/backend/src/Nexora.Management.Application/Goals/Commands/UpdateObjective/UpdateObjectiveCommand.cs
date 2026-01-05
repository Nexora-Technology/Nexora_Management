using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.UpdateObjective;

public record UpdateObjectiveCommand(
    Guid ObjectiveId,
    string? Title,
    string? Description,
    Guid? OwnerId,
    int? Weight,
    string? Status,
    int? PositionOrder
) : IRequest<Result<ObjectiveDto>>;

public class UpdateObjectiveCommandHandler : IRequestHandler<UpdateObjectiveCommand, Result<ObjectiveDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public UpdateObjectiveCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<ObjectiveDto>> Handle(UpdateObjectiveCommand request, CancellationToken ct)
    {
        var objective = await _db.Objectives.FirstOrDefaultAsync(o => o.Id == request.ObjectiveId, ct);
        if (objective == null)
        {
            return Result<ObjectiveDto>.Failure("Objective not found");
        }

        // Update fields if provided
        if (request.Title != null)
            objective.Title = request.Title;

        if (request.Description != null)
            objective.Description = request.Description;

        if (request.OwnerId.HasValue)
            objective.OwnerId = request.OwnerId.Value;

        if (request.Weight.HasValue)
            objective.Weight = request.Weight.Value;

        if (request.Status != null)
            objective.Status = request.Status;

        if (request.PositionOrder.HasValue)
            objective.PositionOrder = request.PositionOrder.Value;

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
