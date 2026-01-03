using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using DomainTask = Nexora.Management.Domain.Entities.Task;

namespace Nexora.Management.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? ParentTaskId,
    Guid? StatusId,
    string Priority,
    Guid? AssigneeId,
    DateTime? DueDate,
    DateTime? StartDate,
    decimal? EstimatedHours
) : IRequest<Result<TaskDto>>;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreateTaskCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        // Validate project exists
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null)
        {
            return Result<TaskDto>.Failure("Project not found");
        }

        // Validate parent task if provided
        if (request.ParentTaskId.HasValue)
        {
            var parentTask = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.ParentTaskId.Value, ct);
            if (parentTask == null || parentTask.ProjectId != request.ProjectId)
            {
                return Result<TaskDto>.Failure("Parent task not found or belongs to different project");
            }
        }

        // Get max position for ordering
        var maxPosition = await _db.Tasks
            .Where(t => t.ProjectId == request.ProjectId && t.ParentTaskId == request.ParentTaskId)
            .MaxAsync(t => (int?)t.PositionOrder) ?? 0;

        var task = new DomainTask
        {
            ProjectId = request.ProjectId,
            ParentTaskId = request.ParentTaskId,
            Title = request.Title,
            Description = request.Description,
            StatusId = request.StatusId,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            DueDate = request.DueDate,
            StartDate = request.StartDate,
            EstimatedHours = request.EstimatedHours,
            PositionOrder = maxPosition + 1,
            CustomFieldsJsonb = new Dictionary<string, object>(),
            CreatedBy = _userContext.UserId
        };

        _db.Tasks.Add(task);
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
