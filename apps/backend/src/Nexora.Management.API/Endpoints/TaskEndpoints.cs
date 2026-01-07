using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nexora.Management.Application.Tasks.Commands.CreateTask;
using Nexora.Management.Application.Tasks.Commands.DeleteTask;
using Nexora.Management.Application.Tasks.Commands.UpdateTask;
using Nexora.Management.Application.Tasks.Commands.UpdateTaskStatus;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Application.Tasks.Queries;
using Nexora.Management.Application.Tasks.Queries.ViewQueries;
using Nexora.Management.API.Hubs;
using Nexora.Management.Application.DTOs.SignalR;
using System.Security.Claims;

namespace Nexora.Management.API.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks")
            .WithTags("Tasks")
            .WithOpenApi();

        // Create task
        group.MapPost("/", async (
            CreateTaskRequest request,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var command = new CreateTaskCommand(
                request.TaskListId,
                request.Title,
                request.Description,
                request.ParentTaskId,
                request.StatusId,
                request.Priority ?? "medium",
                request.AssigneeId,
                request.DueDate,
                request.StartDate,
                request.EstimatedHours
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            // Broadcast TaskCreated to tasklist group
            var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new TaskUpdatedMessage
            {
                TaskId = result.Value.Id,
                TaskListId = request.TaskListId,
                Type = "created",
                UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                Timestamp = DateTime.UtcNow,
                Data = result.Value
            };
            await taskHub.Clients.Group($"tasklist_{request.TaskListId}")
                .SendAsync("TaskCreated", message);

            return Results.Created($"/api/tasks/{result.Value.Id}", result.Value);
        })
        .WithName("CreateTask")
        .WithSummary("Create a new task");

        // Get task by ID
        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetTaskByIdQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTaskById")
        .WithSummary("Get task by ID");

        // Get tasks list
        group.MapGet("/", async (
            [AsParameters] GetTasksQueryRequest request,
            ISender sender) =>
        {
            var query = new GetTasksQuery(
                request.TaskListId,
                request.StatusId,
                request.AssigneeId,
                request.Search,
                request.SortBy,
                request.SortDesc,
                request.Page,
                request.PageSize
            );
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTasks")
        .WithSummary("Get tasks with filters");

        // Update task
        group.MapPut("/{id}", async (
            Guid id,
            UpdateTaskRequest request,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var command = new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.StatusId,
                request.Priority,
                request.AssigneeId,
                request.DueDate,
                request.StartDate,
                request.EstimatedHours
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            // Broadcast TaskUpdated to tasklist group
            var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new TaskUpdatedMessage
            {
                TaskId = id,
                TaskListId = result.Value.TaskListId,
                Type = "updated",
                UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                Timestamp = DateTime.UtcNow,
                Data = result.Value
            };
            await taskHub.Clients.Group($"tasklist_{result.Value.TaskListId}")
                .SendAsync("TaskUpdated", message);

            return Results.Ok(result.Value);
        })
        .WithName("UpdateTask")
        .WithSummary("Update task");

        // Delete task
        group.MapDelete("/{id}", async (
            Guid id,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var command = new DeleteTaskCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            // Broadcast TaskDeleted to tasklist group
            if (result.Value.HasValue)
            {
                var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var message = new TaskUpdatedMessage
                {
                    TaskId = id,
                    TaskListId = result.Value.Value,
                    Type = "deleted",
                    UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                    Timestamp = DateTime.UtcNow,
                    Data = null
                };
                await taskHub.Clients.Group($"tasklist_{result.Value.Value}")
                    .SendAsync("TaskDeleted", message);
            }

            return Results.NoContent();
        })
        .WithName("DeleteTask")
        .WithSummary("Delete task");

        // Update task status (for Board view drag-drop)
        group.MapPatch("/{id}/status", async (
            Guid id,
            [FromBody] UpdateTaskStatusRequest request,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var command = new UpdateTaskStatusCommand(id, request.StatusId);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            // Broadcast TaskStatusChanged to tasklist group
            var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new TaskUpdatedMessage
            {
                TaskId = id,
                TaskListId = result.Value.TaskListId,
                Type = "status_changed",
                UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                Timestamp = DateTime.UtcNow,
                Data = result.Value
            };
            await taskHub.Clients.Group($"tasklist_{result.Value.TaskListId}")
                .SendAsync("TaskStatusChanged", message);

            return Results.Ok(result.Value);
        })
        .WithName("UpdateTaskStatus")
        .WithSummary("Update task status");

        // Board View - Get kanban board data
        group.MapGet("/views/board/{taskListId}", async (Guid taskListId, ISender sender) =>
        {
            var query = new GetBoardViewQuery(taskListId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetBoardView")
        .WithSummary("Get board view data");

        // Calendar View - Get calendar data
        group.MapGet("/views/calendar/{taskListId}", async (Guid taskListId, int year, int month, ISender sender) =>
        {
            var query = new GetCalendarViewQuery(taskListId, year, month);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetCalendarView")
        .WithSummary("Get calendar view data");

        // Gantt View - Get gantt chart data
        group.MapGet("/views/gantt/{taskListId}", async (Guid taskListId, ISender sender) =>
        {
            var query = new GetGanttViewQuery(taskListId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetGanttView")
        .WithSummary("Get gantt view data");
    }
}

public record UpdateTaskStatusRequest(Guid StatusId);
