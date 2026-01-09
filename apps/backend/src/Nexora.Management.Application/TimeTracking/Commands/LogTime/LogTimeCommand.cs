using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Commands.LogTime;

public record LogTimeCommand(
    Guid? TaskId,
    DateTime StartTime,
    DateTime? EndTime,
    int DurationMinutes,
    string? Description,
    bool IsBillable,
    Guid? WorkspaceId
) : IRequest<Result<TimeEntryDto>>;

public class LogTimeCommandHandler : IRequestHandler<LogTimeCommand, Result<TimeEntryDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public LogTimeCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<TimeEntryDto>> Handle(LogTimeCommand request, CancellationToken ct)
    {
        // Validate duration
        if (request.DurationMinutes <= 0)
        {
            return Result<TimeEntryDto>.Failure("Duration must be greater than 0");
        }

        // Validate description length
        if (request.Description != null && request.Description.Length > 1000)
        {
            return Result<TimeEntryDto>.Failure("Description cannot exceed 1000 characters");
        }

        // Validate date ranges
        if (request.EndTime.HasValue && request.EndTime < request.StartTime)
        {
            return Result<TimeEntryDto>.Failure("EndTime cannot be before StartTime");
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

        // Calculate duration if not provided
        var duration = request.DurationMinutes;
        if (duration == 0 && request.EndTime.HasValue)
        {
            duration = (int)(request.EndTime.Value - request.StartTime).TotalMinutes;
        }

        var entry = new TimeEntry
        {
            UserId = _userContext.UserId,
            TaskId = request.TaskId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DurationMinutes = duration,
            Description = request.Description,
            IsBillable = request.IsBillable,
            Status = "draft",
            WorkspaceId = workspaceId
        };

        _db.TimeEntries.Add(entry);
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
