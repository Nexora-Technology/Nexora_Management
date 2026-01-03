using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Tasks.Commands.CreateTask;
using Nexora.Management.Application.Tasks.Commands.DeleteTask;
using Nexora.Management.Application.Tasks.Commands.UpdateTask;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Application.Tasks.Queries;

namespace Nexora.Management.API.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks")
            .WithTags("Tasks")
            .WithOpenApi();

        // Create task
        group.MapPost("/", async (CreateTaskRequest request, ISender sender) =>
        {
            var command = new CreateTaskCommand(
                request.ProjectId,
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
                request.ProjectId,
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
        group.MapPut("/{id}", async (Guid id, UpdateTaskRequest request, ISender sender) =>
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

            return Results.Ok(result.Value);
        })
        .WithName("UpdateTask")
        .WithSummary("Update task");

        // Delete task
        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteTaskCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteTask")
        .WithSummary("Delete task");
    }
}
