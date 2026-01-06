using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TaskLists.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TaskLists.Queries.GetTaskListById;

public record GetTaskListByIdQuery(Guid Id) : IRequest<Result<TaskListDto>>;

public class GetTaskListByIdQueryHandler : IRequestHandler<GetTaskListByIdQuery, Result<TaskListDto>>
{
    private readonly IAppDbContext _db;

    public GetTaskListByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TaskListDto>> Handle(GetTaskListByIdQuery request, CancellationToken ct)
    {
        var taskList = await _db.TaskLists
            .AsNoTracking()
            .FirstOrDefaultAsync(tl => tl.Id == request.Id, ct);

        if (taskList == null)
        {
            return Result<TaskListDto>.Failure("TaskList not found");
        }

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
