using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Queries.GetObjectiveTree;

public record GetObjectiveTreeQuery(
    Guid WorkspaceId,
    Guid? PeriodId
) : IRequest<Result<List<ObjectiveTreeNodeDto>>>;

public class GetObjectiveTreeQueryHandler : IRequestHandler<GetObjectiveTreeQuery, Result<List<ObjectiveTreeNodeDto>>>
{
    private readonly IAppDbContext _db;

    public GetObjectiveTreeQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<ObjectiveTreeNodeDto>>> Handle(GetObjectiveTreeQuery request, CancellationToken ct)
    {
        var query = _db.Objectives
            .Include(o => o.Owner)
            .Include(o => o.KeyResults)
            .Where(o => o.WorkspaceId == request.WorkspaceId);

        if (request.PeriodId.HasValue)
        {
            query = query.Where(o => o.PeriodId == request.PeriodId.Value);
        }

        var objectives = await query
            .OrderBy(o => o.PositionOrder)
            .ToListAsync(ct);

        // Build tree structure (3 levels max)
        var rootObjectives = objectives.Where(o => o.ParentObjectiveId == null).ToList();
        var tree = BuildTree(rootObjectives, objectives, 0);

        return Result<List<ObjectiveTreeNodeDto>>.Success(tree);
    }

    private List<ObjectiveTreeNodeDto> BuildTree(List<Objective> parentObjectives, List<Objective> allObjectives, int currentLevel)
    {
        if (currentLevel >= 3) // Max 3 levels
        {
            return new List<ObjectiveTreeNodeDto>();
        }

        var nodes = new List<ObjectiveTreeNodeDto>();

        foreach (var parent in parentObjectives)
        {
            var children = allObjectives.Where(o => o.ParentObjectiveId == parent.Id).ToList();

            var node = new ObjectiveTreeNodeDto(
                parent.Id,
                parent.WorkspaceId,
                parent.PeriodId,
                parent.ParentObjectiveId,
                parent.Title,
                parent.Description,
                parent.OwnerId,
                parent.Owner?.Name,
                parent.Owner?.Email,
                parent.Weight,
                parent.Status,
                parent.Progress,
                parent.PositionOrder,
                parent.CreatedAt,
                parent.UpdatedAt,
                parent.KeyResults.Select(kr => new KeyResultDto(
                    kr.Id,
                    kr.ObjectiveId,
                    kr.Title,
                    kr.MetricType,
                    kr.CurrentValue,
                    kr.TargetValue,
                    kr.Unit,
                    kr.DueDate,
                    kr.Progress,
                    kr.Weight,
                    kr.CreatedAt,
                    kr.UpdatedAt
                )).ToList(),
                BuildTree(children, allObjectives, currentLevel + 1)
            );

            nodes.Add(node);
        }

        return nodes;
    }
}
