using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.TaskLists.Commands.CreateTaskList;
using Nexora.Management.Application.TaskLists.Commands.DeleteTaskList;
using Nexora.Management.Application.TaskLists.Commands.UpdateTaskList;
using Nexora.Management.Application.TaskLists.Commands.UpdateTaskListPosition;
using Nexora.Management.Application.TaskLists.DTOs;
using Nexora.Management.Application.TaskLists.Queries.GetTaskListById;
using Nexora.Management.Application.TaskLists.Queries.GetTaskLists;

namespace Nexora.Management.API.Endpoints;

public static class TaskListEndpoints
{
    public static void MapTaskListEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasklists")
            .WithTags("TaskLists")
            .WithOpenApi();

        // Create tasklist
        group.MapPost("/", async (
            CreateTaskListRequest request,
            ISender sender) =>
        {
            var command = new CreateTaskListCommand(
                request.SpaceId,
                request.FolderId,
                request.Name,
                request.Description,
                request.Color,
                request.Icon,
                request.ListType,
                request.OwnerId
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/tasklists/{result.Value.Id}", result.Value);
        })
        .WithName("CreateTaskList")
        .WithSummary("Create a new tasklist")
        .WithDescription("Creates a new tasklist in the specified space or folder");

        // Get tasklist by ID
        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetTaskListByIdQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTaskListById")
        .WithSummary("Get tasklist by ID")
        .WithDescription("Retrieves a single tasklist by its ID");

        // Get tasklists
        group.MapGet("/", async ([AsParameters] GetTaskListsRequest request, ISender sender) =>
        {
            var query = new GetTaskListsQuery(request.SpaceId, request.FolderId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTaskLists")
        .WithSummary("Get tasklists")
        .WithDescription("Retrieves tasklists filtered by space and/or folder");

        // Update tasklist
        group.MapPut("/{id}", async (
            Guid id,
            UpdateTaskListRequest request,
            ISender sender) =>
        {
            var command = new UpdateTaskListCommand(
                id,
                request.Name,
                request.Description,
                request.Color,
                request.Icon,
                request.Status
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateTaskList")
        .WithSummary("Update tasklist")
        .WithDescription("Updates an existing tasklist");

        // Update tasklist position
        group.MapPatch("/{id}/position", async (
            Guid id,
            UpdateTaskListPositionRequest request,
            ISender sender) =>
        {
            var command = new UpdateTaskListPositionCommand(id, request.PositionOrder);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateTaskListPosition")
        .WithSummary("Update tasklist position")
        .WithDescription("Updates the position order of a tasklist");

        // Delete tasklist
        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteTaskListCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteTaskList")
        .WithSummary("Delete tasklist")
        .WithDescription("Deletes a tasklist (cascades to tasks)");
    }
}
