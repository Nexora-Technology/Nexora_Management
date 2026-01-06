namespace Nexora.Management.Application.TaskLists.DTOs;

public record TaskListDto(
    Guid Id,
    Guid SpaceId,
    Guid? FolderId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    string ListType,
    string Status,
    Guid OwnerId,
    int PositionOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTaskListRequest(
    Guid SpaceId,
    Guid? FolderId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    string ListType = "task",
    Guid? OwnerId = null
);

public record UpdateTaskListRequest(
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    string Status
);

public record UpdateTaskListPositionRequest(
    int PositionOrder
);

public record GetTaskListsRequest(
    Guid? SpaceId,
    Guid? FolderId
);
