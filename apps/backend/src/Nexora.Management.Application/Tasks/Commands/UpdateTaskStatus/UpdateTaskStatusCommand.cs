using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(
    Guid TaskId,
    Guid StatusId
) : IRequest<Result<TaskDto>>;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _db;

    public UpdateTaskStatusCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(
        UpdateTaskStatusCommand request,
        CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId, ct);
        if (task == null)
        {
            return Result<TaskDto>.Failure("Task not found");
        }

        // Update status
        task.StatusId = request.StatusId;
        await _db.SaveChangesAsync(ct);

        var taskDto = new TaskDto(
            task.Id,
            task.ProjectId,
            task.ParentTaskId,
            task.Title,
            task.Description,
            task.StatusId,
            task.Priority,
            task.AssigneeId,
            task.DueDate,
            task.StartDate,
            task.EstimatedHours,
            task.PositionOrder,
            task.CreatedBy,
            task.CreatedAt,
            task.UpdatedAt
        );

        return Result<TaskDto>.Success(taskDto);
    }
}
