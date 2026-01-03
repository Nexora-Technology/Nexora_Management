namespace Nexora.Management.Application.Tasks.DTOs;

public record TaskDto(
    Guid Id,
    Guid ProjectId,
    Guid? ParentTaskId,
    string Title,
    string? Description,
    Guid? StatusId,
    string Priority,
    Guid? AssigneeId,
    DateTime? DueDate,
    DateTime? StartDate,
    decimal? EstimatedHours,
    int PositionOrder,
    Guid CreatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTaskRequest(
    Guid ProjectId,
    string Title,
    string? Description = null,
    Guid? ParentTaskId = null,
    Guid? StatusId = null,
    string? Priority = null,
    Guid? AssigneeId = null,
    DateTime? DueDate = null,
    DateTime? StartDate = null,
    decimal? EstimatedHours = null
);

public record UpdateTaskRequest(
    string Title,
    string? Description = null,
    Guid? StatusId = null,
    string? Priority = null,
    Guid? AssigneeId = null,
    DateTime? DueDate = null,
    DateTime? StartDate = null,
    decimal? EstimatedHours = null
);

public record GetTasksQueryRequest(
    Guid? ProjectId = null,
    Guid? StatusId = null,
    Guid? AssigneeId = null,
    string? Search = null,
    string? SortBy = null,
    bool SortDesc = false,
    int Page = 1,
    int PageSize = 20
);
