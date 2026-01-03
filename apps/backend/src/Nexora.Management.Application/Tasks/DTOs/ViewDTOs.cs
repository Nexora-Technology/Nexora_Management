namespace Nexora.Management.Application.Tasks.DTOs;

// Board View DTOs
public record BoardColumnDto(
    string Id,
    string Name,
    string? Color,
    int OrderIndex,
    List<TaskDto> Tasks
);

// Calendar View DTOs
public record CalendarTaskDto(
    Guid Id,
    string Title,
    DateTime? DueDate,
    Guid? StatusId,
    string Priority,
    Guid? AssigneeId
);

// Gantt View DTOs
public record GanttTaskDto(
    Guid Id,
    string Title,
    DateTime? StartDate,
    DateTime? DueDate,
    int Duration,
    Guid? StatusId,
    string Priority,
    Guid? ParentTaskId,
    List<GanttTaskDto> Children
);

// List View already uses existing TaskDto with GetTasksQuery
