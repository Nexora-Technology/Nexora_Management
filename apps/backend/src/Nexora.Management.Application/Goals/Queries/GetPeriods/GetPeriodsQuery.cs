using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Queries.GetPeriods;

public record GetPeriodsQuery(
    Guid WorkspaceId,
    string? Status
) : IRequest<Result<List<GoalPeriodDto>>>;

public class GetPeriodsQueryHandler : IRequestHandler<GetPeriodsQuery, Result<List<GoalPeriodDto>>>
{
    private readonly IAppDbContext _db;

    public GetPeriodsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<GoalPeriodDto>>> Handle(GetPeriodsQuery request, CancellationToken ct)
    {
        var query = _db.GoalPeriods
            .Where(p => p.WorkspaceId == request.WorkspaceId);

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(p => p.Status == request.Status);
        }

        var periods = await query
            .OrderByDescending(p => p.StartDate)
            .Select(p => new GoalPeriodDto(
                p.Id,
                p.WorkspaceId,
                p.Name,
                p.StartDate,
                p.EndDate,
                p.Status,
                p.CreatedAt,
                p.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<GoalPeriodDto>>.Success(periods);
    }
}
