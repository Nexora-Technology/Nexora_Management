using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TaskLists.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TaskLists.Commands.UpdateTaskListPosition;

public record UpdateTaskListPositionCommand(Guid Id, int PositionOrder) : IRequest<Result<TaskListDto>>;

public class UpdateTaskListPositionCommandHandler : IRequestHandler<UpdateTaskListPositionCommand, Result<TaskListDto>>
{
    private readonly IAppDbContext _db;

    public UpdateTaskListPositionCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TaskListDto>> Handle(UpdateTaskListPositionCommand request, CancellationToken ct)
    {
        var taskList = await _db.TaskLists.FirstOrDefaultAsync(tl => tl.Id == request.Id, ct);
        if (taskList == null)
        {
            return Result<TaskListDto>.Failure("TaskList not found");
        }

        taskList.PositionOrder = request.PositionOrder;
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
