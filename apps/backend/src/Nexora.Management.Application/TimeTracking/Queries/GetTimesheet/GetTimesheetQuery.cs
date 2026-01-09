using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Queries.GetTimesheet;

public record GetTimesheetQuery(
    Guid UserId,
    DateTime WeekStart
) : IRequest<Result<TimesheetDto>>;

public class GetTimesheetQueryHandler : IRequestHandler<GetTimesheetQuery, Result<TimesheetDto>>
{
    private readonly IAppDbContext _db;

    public GetTimesheetQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TimesheetDto>> Handle(GetTimesheetQuery request, CancellationToken ct)
    {
        // Calculate week end (7 days from start)
        var weekEnd = request.WeekStart.AddDays(7);

        // Get all entries for the week
        var entries = await _db.TimeEntries
            .Where(te => te.UserId == request.UserId
                && te.StartTime >= request.WeekStart
                && te.StartTime < weekEnd)
            .OrderBy(te => te.StartTime)
            .Select(te => new TimeEntryDto(
                te.Id,
                te.UserId,
                te.TaskId,
                te.StartTime,
                te.EndTime,
                te.DurationMinutes,
                te.Description,
                te.IsBillable,
                te.Status,
                te.WorkspaceId,
                te.CreatedAt,
                te.UpdatedAt
            ))
            .ToListAsync(ct);

        // Group by day
        var dailyTotals = new List<DailyTimeDto>();
        var totalMinutes = 0;

        for (var day = 0; day < 7; day++)
        {
            var currentDay = request.WeekStart.AddDays(day);
            var nextDay = currentDay.AddDays(1);

            var dayEntries = entries
                .Where(e => e.StartTime >= currentDay && e.StartTime < nextDay)
                .ToList();

            var dayTotal = dayEntries.Sum(e => e.DurationMinutes);
            var billableTotal = dayEntries.Where(e => e.IsBillable).Sum(e => e.DurationMinutes);

            dailyTotals.Add(new DailyTimeDto(
                currentDay,
                dayTotal,
                billableTotal,
                dayEntries
            ));

            totalMinutes += dayTotal;
        }

        var timesheet = new TimesheetDto(
            request.UserId,
            request.WeekStart,
            weekEnd,
            dailyTotals,
            totalMinutes
        );

        return Result<TimesheetDto>.Success(timesheet);
    }
}
