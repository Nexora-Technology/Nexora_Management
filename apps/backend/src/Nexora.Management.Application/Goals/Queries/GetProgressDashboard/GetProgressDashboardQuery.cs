using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Queries.GetProgressDashboard;

public record GetProgressDashboardQuery(
    Guid WorkspaceId,
    Guid? PeriodId
) : IRequest<Result<ProgressDashboardDto>>;

public class GetProgressDashboardQueryHandler : IRequestHandler<GetProgressDashboardQuery, Result<ProgressDashboardDto>>
{
    private readonly IAppDbContext _db;

    public GetProgressDashboardQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<ProgressDashboardDto>> Handle(GetProgressDashboardQuery request, CancellationToken ct)
    {
        var objectivesQuery = _db.Objectives
            .Include(o => o.KeyResults)
            .Where(o => o.WorkspaceId == request.WorkspaceId);

        if (request.PeriodId.HasValue)
        {
            objectivesQuery = objectivesQuery.Where(o => o.PeriodId == request.PeriodId.Value);
        }

        var objectives = await objectivesQuery.ToListAsync(ct);

        var totalObjectives = objectives.Count;
        var onTrackObjectives = objectives.Count(o => o.Status == "on-track");
        var atRiskObjectives = objectives.Count(o => o.Status == "at-risk");
        var offTrackObjectives = objectives.Count(o => o.Status == "off-track");
        var completedObjectives = objectives.Count(o => o.Status == "completed");

        var averageProgress = totalObjectives > 0
            ? (int)objectives.Average(o => o.Progress)
            : 0;

        var totalKeyResults = objectives.Sum(o => o.KeyResults.Count);

        var completedKeyResults = objectives
            .Sum(o => o.KeyResults.Count(kr => kr.Progress >= 100));

        // Breakdown by status
        var statusBreakdown = new List<StatusBreakdownDto>
        {
            new("on-track", onTrackObjectives, onTrackObjectives * 100.0 / Math.Max(1, totalObjectives)),
            new("at-risk", atRiskObjectives, atRiskObjectives * 100.0 / Math.Max(1, totalObjectives)),
            new("off-track", offTrackObjectives, offTrackObjectives * 100.0 / Math.Max(1, totalObjectives)),
            new("completed", completedObjectives, completedObjectives * 100.0 / Math.Max(1, totalObjectives))
        };

        // Top 5 objectives by progress
        var topObjectives = objectives
            .OrderByDescending(o => o.Progress)
            .Take(5)
            .Select(o => new ObjectiveSummaryDto(
                o.Id,
                o.Title,
                o.Status,
                o.Progress,
                o.KeyResults.Count,
                o.OwnerId
            ))
            .ToList();

        // Bottom 5 objectives by progress (excluding completed)
        var bottomObjectives = objectives
            .Where(o => o.Status != "completed")
            .OrderBy(o => o.Progress)
            .Take(5)
            .Select(o => new ObjectiveSummaryDto(
                o.Id,
                o.Title,
                o.Status,
                o.Progress,
                o.KeyResults.Count,
                o.OwnerId
            ))
            .ToList();

        var dashboard = new ProgressDashboardDto(
            totalObjectives,
            averageProgress,
            totalKeyResults,
            completedKeyResults,
            statusBreakdown,
            topObjectives,
            bottomObjectives
        );

        return Result<ProgressDashboardDto>.Success(dashboard);
    }
}
