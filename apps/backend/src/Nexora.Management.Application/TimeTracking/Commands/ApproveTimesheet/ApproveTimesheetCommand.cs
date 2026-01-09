using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Commands.ApproveTimesheet;

public record ApproveTimesheetCommand(
    Guid UserId,
    DateTime WeekStart,
    DateTime WeekEnd,
    string Status // "approved" or "rejected"
) : IRequest<Result>;

public class ApproveTimesheetCommandHandler : IRequestHandler<ApproveTimesheetCommand, Result>
{
    private readonly IAppDbContext _db;

    public ApproveTimesheetCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(ApproveTimesheetCommand request, CancellationToken ct)
    {
        if (request.Status != "approved" && request.Status != "rejected")
        {
            return Result.Failure("Status must be 'approved' or 'rejected'");
        }

        // Find all time entries for the week
        var weekEnd = request.WeekEnd.AddDays(1).AddSeconds(-1); // End of the day

        var entries = await _db.TimeEntries
            .Where(te => te.UserId == request.UserId
                && te.StartTime >= request.WeekStart
                && te.StartTime <= weekEnd
                && te.Status == "submitted")
            .ToListAsync(ct);

        if (!entries.Any())
        {
            return Result.Failure("No submitted time entries found for the specified period");
        }

        // Update status
        foreach (var entry in entries)
        {
            entry.Status = request.Status;
        }

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
