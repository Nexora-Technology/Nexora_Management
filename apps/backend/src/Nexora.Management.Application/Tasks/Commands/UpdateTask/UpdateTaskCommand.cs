using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using DomainTask = Nexora.Management.Domain.Entities.Task;

namespace Nexora.Management.Application.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string? Description,
    Guid? StatusId,
    string? Priority,
    Guid? AssigneeId,
    DateTime? DueDate,
    DateTime? StartDate,
    decimal? EstimatedHours
) : IRequest<Result<TaskDto>>;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _db;

    public UpdateTaskCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (task == null)
        {
            return Result<TaskDto>.Failure("Task not found");
        }

        // Update fields
        task.Title = request.Title;
        task.Description = request.Description;
        task.StatusId = request.StatusId ?? task.StatusId;
        task.Priority = request.Priority ?? task.Priority;
        task.AssigneeId = request.AssigneeId;
        task.DueDate = request.DueDate;
        task.StartDate = request.StartDate;
        task.EstimatedHours = request.EstimatedHours;

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
