using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Commands.StartTime;

public record StartTimeCommand(
    Guid? TaskId,
    string? Description,
    bool IsBillable,
    Guid? WorkspaceId
) : IRequest<Result<TimeEntryDto>>;

public class StartTimeCommandHandler : IRequestHandler<StartTimeCommand, Result<TimeEntryDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public StartTimeCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<TimeEntryDto>> Handle(StartTimeCommand request, CancellationToken ct)
    {
        // Check if user has an active timer
        var activeEntry = await _db.TimeEntries
            .FirstOrDefaultAsync(te => te.UserId == _userContext.UserId && te.EndTime == null, ct);

        if (activeEntry != null)
        {
            return Result<TimeEntryDto>.Failure("User already has an active timer. Stop it before starting a new one.");
        }

        // Validate task exists if provided
        if (request.TaskId.HasValue)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId.Value, ct);
            if (task == null)
            {
                return Result<TimeEntryDto>.Failure("Task not found");
            }
        }

        // Determine workspace ID
        var workspaceId = request.WorkspaceId;
        if (!workspaceId.HasValue && request.TaskId.HasValue)
        {
            var task = await _db.Tasks
                .Include(t => t.TaskList)
                .ThenInclude(tl => tl!.Space)
                .FirstOrDefaultAsync(t => t.Id == request.TaskId.Value, ct);

            if (task?.TaskList?.Space?.WorkspaceId != null)
            {
                workspaceId = task.TaskList.Space.WorkspaceId;
            }
        }

        var entry = new TimeEntry
        {
            UserId = _userContext.UserId,
            TaskId = request.TaskId,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            DurationMinutes = 0,
            Description = request.Description,
            IsBillable = request.IsBillable,
            Status = "draft",
            WorkspaceId = workspaceId
        };

        _db.TimeEntries.Add(entry);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique constraint") == true
            || ex.InnerException?.Message.Contains("duplicate key") == true)
        {
            // Race condition: Another timer was started concurrently
            return Result<TimeEntryDto>.Failure("A timer was already started. Please refresh and try again.");
        }

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
