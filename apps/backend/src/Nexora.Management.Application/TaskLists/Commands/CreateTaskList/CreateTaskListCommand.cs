using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TaskLists.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TaskLists.Commands.CreateTaskList;

public record CreateTaskListCommand(
    Guid SpaceId,
    Guid? FolderId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    string ListType = "task",
    Guid? OwnerId = null
) : IRequest<Result<TaskListDto>>;

public class CreateTaskListCommandHandler : IRequestHandler<CreateTaskListCommand, Result<TaskListDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreateTaskListCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<TaskListDto>> Handle(CreateTaskListCommand request, CancellationToken ct)
    {
        // Validate space exists
        var space = await _db.Spaces.FirstOrDefaultAsync(s => s.Id == request.SpaceId, ct);
        if (space == null)
        {
            return Result<TaskListDto>.Failure("Space not found");
        }

        // Validate folder if provided
        if (request.FolderId.HasValue)
        {
            var folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId.Value, ct);
            if (folder == null || folder.SpaceId != request.SpaceId)
            {
                return Result<TaskListDto>.Failure("Folder not found or does not belong to the specified space");
            }
        }

        // Get max position for ordering
        var maxPosition = await _db.TaskLists
            .Where(tl => tl.SpaceId == request.SpaceId && tl.FolderId == request.FolderId)
            .MaxAsync(tl => (int?)tl.PositionOrder) ?? 0;

        var taskList = new TaskList
        {
            SpaceId = request.SpaceId,
            FolderId = request.FolderId,
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            ListType = request.ListType,
            Status = "active",
            OwnerId = request.OwnerId ?? _userContext.UserId,
            PositionOrder = maxPosition + 1,
            SettingsJsonb = new Dictionary<string, object>()
        };

        _db.TaskLists.Add(taskList);
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
