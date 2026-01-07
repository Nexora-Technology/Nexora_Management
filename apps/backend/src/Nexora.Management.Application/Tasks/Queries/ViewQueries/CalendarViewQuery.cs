using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Tasks.Queries.ViewQueries;

public record GetCalendarViewQuery(Guid TaskListId, int Year, int Month) : IRequest<Result<List<CalendarTaskDto>>>;

public class GetCalendarViewQueryHandler : IRequestHandler<GetCalendarViewQuery, Result<List<CalendarTaskDto>>>
{
    private readonly IAppDbContext _db;

    public GetCalendarViewQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<CalendarTaskDto>>> Handle(
        GetCalendarViewQuery request,
        CancellationToken ct)
    {
        // Calculate date range for the month
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Get tasks with due dates in the specified month
        var tasks = await _db.Tasks
            .Where(t => t.TaskListId == request.TaskListId)
            .Where(t => t.DueDate.HasValue &&
                        t.DueDate.Value >= startDate &&
                        t.DueDate.Value <= endDate)
            .OrderBy(t => t.DueDate)
            .Select(t => new CalendarTaskDto(
                t.Id,
                t.Title,
                t.DueDate,
                t.StatusId,
                t.Priority,
                t.AssigneeId
            ))
            .ToListAsync(ct);

        return Result<List<CalendarTaskDto>>.Success(tasks);
    }
}
