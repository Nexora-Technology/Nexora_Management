using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TaskLists.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TaskLists.Commands.UpdateTaskList;

public record UpdateTaskListCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    string Status
) : IRequest<Result<TaskListDto>>;

public class UpdateTaskListCommandHandler : IRequestHandler<UpdateTaskListCommand, Result<TaskListDto>>
{
    private readonly IAppDbContext _db;

    public UpdateTaskListCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TaskListDto>> Handle(UpdateTaskListCommand request, CancellationToken ct)
    {
        var taskList = await _db.TaskLists.FirstOrDefaultAsync(tl => tl.Id == request.Id, ct);
        if (taskList == null)
        {
            return Result<TaskListDto>.Failure("TaskList not found");
        }

        taskList.Name = request.Name;
        taskList.Description = request.Description;
        taskList.Color = request.Color;
        taskList.Icon = request.Icon;
        taskList.Status = request.Status;

        await _db.SaveChangesAsync(ct);

        var taskListDto = new TaskListDto(
            taskList.Id,
            taskList.SpaceId,
            taskList.FolderId,
            taskList.Name,
            taskList.Description,
            taskList.Color,
            taskList.Icon,
            taskList.ListType,
            taskList.Status,
            taskList.OwnerId,
            taskList.PositionOrder,
            taskList.CreatedAt,
            taskList.UpdatedAt
        );

        return Result<TaskListDto>.Success(taskListDto);
    }
}
