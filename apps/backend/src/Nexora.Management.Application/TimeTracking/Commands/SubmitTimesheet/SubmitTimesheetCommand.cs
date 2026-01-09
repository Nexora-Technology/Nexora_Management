using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Commands.SubmitTimesheet;

public record SubmitTimesheetCommand(
    Guid UserId,
    DateTime WeekStart,
    DateTime WeekEnd
) : IRequest<Result>;

public class SubmitTimesheetCommandHandler : IRequestHandler<SubmitTimesheetCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public SubmitTimesheetCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result> Handle(SubmitTimesheetCommand request, CancellationToken ct)
    {
        // Authorization: Users can only submit their own timesheets
        if (request.UserId != _userContext.UserId)
        {
            return Result.Failure("You can only submit your own timesheets");
        }

        // Find all draft time entries for the week
        var weekEnd = request.WeekEnd.AddDays(1).AddSeconds(-1); // End of the day

        var entries = await _db.TimeEntries
            .Where(te => te.UserId == request.UserId
                && te.StartTime >= request.WeekStart
                && te.StartTime <= weekEnd
                && te.Status == "draft")
            .ToListAsync(ct);

        if (!entries.Any())
        {
            return Result.Failure("No draft time entries found for the specified period");
        }

        // Update status to submitted
        foreach (var entry in entries)
        {
            entry.Status = "submitted";
        }

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
