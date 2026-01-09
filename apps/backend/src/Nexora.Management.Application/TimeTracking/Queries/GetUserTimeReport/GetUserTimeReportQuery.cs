using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Queries.GetUserTimeReport;

public record GetUserTimeReportQuery(
    Guid UserId,
    DateTime PeriodStart,
    DateTime PeriodEnd
) : IRequest<Result<TimeReportDto>>;

public class GetUserTimeReportQueryHandler : IRequestHandler<GetUserTimeReportQuery, Result<TimeReportDto>>
{
    private readonly IAppDbContext _db;

    public GetUserTimeReportQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TimeReportDto>> Handle(GetUserTimeReportQuery request, CancellationToken ct)
    {
        var periodEnd = request.PeriodEnd.AddDays(1).AddSeconds(-1);

        // Get all entries for the period
        var entries = await _db.TimeEntries
            .Where(te => te.UserId == request.UserId
                && te.StartTime >= request.PeriodStart
                && te.StartTime <= periodEnd)
            .Include(te => te.Task)
            .ToListAsync(ct);

        // Get all applicable rates for the user and projects
        var projectIds = entries.Where(e => e.TaskId.HasValue).Select(e => e.Task!.ProjectId).Distinct().ToList();
        var rates = await _db.TimeRates
            .Where(r => (r.UserId == null || r.UserId == request.UserId)
                && (r.ProjectId == null || projectIds.Contains(r.ProjectId.Value))
                && r.EffectiveFrom <= periodEnd
                && (r.EffectiveTo == null || r.EffectiveTo >= request.PeriodStart))
            .ToListAsync(ct);

        var totalMinutes = entries.Sum(e => e.DurationMinutes);
        var billableMinutes = entries.Where(e => e.IsBillable).Sum(e => e.DurationMinutes);

        // Calculate amount based on rates (use highest rate for each entry)
        decimal totalAmount = 0;
        foreach (var entry in entries.Where(e => e.IsBillable))
        {
            var applicableRates = rates
                .Where(r => r.EffectiveFrom <= entry.StartTime
                    && (r.EffectiveTo == null || r.EffectiveTo >= entry.StartTime)
                    && (r.UserId == null || r.UserId == entry.UserId)
                    && (r.ProjectId == null || (entry.Task?.ProjectId != null && r.ProjectId == entry.Task.ProjectId)))
                .ToList();

            // Use highest rate if multiple rates match
            var rate = applicableRates.Any() ? applicableRates.Max(r => r.HourlyRate) : 50m; // Fallback to default
            totalAmount += (entry.DurationMinutes / 60m) * rate;
        }

        // Group by task
        var taskBreakdown = entries
            .Where(e => e.TaskId.HasValue)
            .GroupBy(e => e.TaskId)
            .Select(g => new TaskTimeBreakdownDto(
                g.Key,
                g.FirstOrDefault()?.Task?.Title,
                g.Sum(e => e.DurationMinutes),
                g.Count()
            ))
            .OrderByDescending(b => b.TotalMinutes)
            .ToList();

        var report = new TimeReportDto(
            request.UserId,
            request.PeriodStart,
            request.PeriodEnd,
            totalMinutes,
            billableMinutes,
            totalAmount,
            taskBreakdown
        );

        return Result<TimeReportDto>.Success(report);
    }
}
