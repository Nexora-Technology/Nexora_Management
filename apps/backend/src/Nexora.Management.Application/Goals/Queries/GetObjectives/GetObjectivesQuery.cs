using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Queries.GetObjectives;

public record GetObjectivesQuery(
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string? Status,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<ObjectiveDto>>>;

public class GetObjectivesQueryHandler : IRequestHandler<GetObjectivesQuery, Result<PagedResult<ObjectiveDto>>>
{
    private readonly IAppDbContext _db;

    public GetObjectivesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<PagedResult<ObjectiveDto>>> Handle(GetObjectivesQuery request, CancellationToken ct)
    {
        var query = _db.Objectives
            .Where(o => o.WorkspaceId == request.WorkspaceId);

        if (request.PeriodId.HasValue)
        {
            query = query.Where(o => o.PeriodId == request.PeriodId.Value);
        }

        if (request.ParentObjectiveId.HasValue)
        {
            query = query.Where(o => o.ParentObjectiveId == request.ParentObjectiveId.Value);
        }
        else
        {
            // If null, get root objectives only (no parent)
            query = query.Where(o => o.ParentObjectiveId == null);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(o => o.Status == request.Status);
        }

        var total = await query.CountAsync(ct);

        var objectives = await query
            .OrderBy(o => o.PositionOrder)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new ObjectiveDto(
                o.Id,
                o.WorkspaceId,
                o.PeriodId,
                o.ParentObjectiveId,
                o.Title,
                o.Description,
                o.OwnerId,
                o.Weight,
                o.Status,
                o.Progress,
                o.PositionOrder,
                o.CreatedAt,
                o.UpdatedAt
            ))
            .ToListAsync(ct);

        var pagedResult = new PagedResult<ObjectiveDto>(
            objectives,
            total,
            request.Page,
            request.PageSize
        );

        return Result<PagedResult<ObjectiveDto>>.Success(pagedResult);
    }
}
