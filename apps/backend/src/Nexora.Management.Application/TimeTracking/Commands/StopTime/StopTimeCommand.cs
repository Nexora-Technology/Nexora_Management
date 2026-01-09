using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Commands.StopTime;

public record StopTimeCommand(
    string? Description
) : IRequest<Result<TimeEntryDto>>;

public class StopTimeCommandHandler : IRequestHandler<StopTimeCommand, Result<TimeEntryDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public StopTimeCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<TimeEntryDto>> Handle(StopTimeCommand request, CancellationToken ct)
    {
        // Find active timer
        var entry = await _db.TimeEntries
            .FirstOrDefaultAsync(te => te.UserId == _userContext.UserId && te.EndTime == null, ct);

        if (entry == null)
        {
            return Result<TimeEntryDto>.Failure("No active timer found");
        }

        // Stop the timer
        entry.EndTime = DateTime.UtcNow;
        entry.DurationMinutes = (int)(entry.EndTime.Value - entry.StartTime).TotalMinutes;

        if (!string.IsNullOrEmpty(request.Description))
        {
            entry.Description = request.Description;
        }

        await _db.SaveChangesAsync(ct);

        var entryDto = new TimeEntryDto(
            entry.Id,
            entry.UserId,
            entry.TaskId,
            entry.StartTime,
            entry.EndTime,
            entry.DurationMinutes,
            entry.Description,
            entry.IsBillable,
            entry.Status,
            entry.WorkspaceId,
            entry.CreatedAt,
            entry.UpdatedAt
        );

        return Result<TimeEntryDto>.Success(entryDto);
    }
}
