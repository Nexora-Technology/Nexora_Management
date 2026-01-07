using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Folders.Commands.CreateFolder;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Application.Folders.Queries.GetFoldersBySpace;
using Nexora.Management.Application.Spaces.Commands.CreateSpace;
using Nexora.Management.Application.Spaces.Commands.DeleteSpace;
using Nexora.Management.Application.Spaces.Commands.UpdateSpace;
using Nexora.Management.Application.Spaces.DTOs;
using Nexora.Management.Application.Spaces.Queries.GetSpaceById;
using Nexora.Management.Application.Spaces.Queries.GetSpacesByWorkspace;
using Nexora.Management.Application.TaskLists.Commands.CreateTaskList;
using Nexora.Management.Application.TaskLists.DTOs;

namespace Nexora.Management.API.Endpoints;

public static class SpaceEndpoints
{
    public static void MapSpaceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/spaces")
            .WithTags("Spaces")
            .WithOpenApi();

        // Create space
        group.MapPost("/", async (
            CreateSpaceRequest request,
            ISender sender) =>
        {
            var command = new CreateSpaceCommand(
                request.WorkspaceId,
                request.Name,
                request.Description,
                request.Color,
                request.Icon,
                request.IsPrivate
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/spaces/{result.Value.Id}", result.Value);
        })
        .WithName("CreateSpace")
        .WithSummary("Create a new space")
        .WithDescription("Creates a new space in the specified workspace");

        // Get space by ID
        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetSpaceByIdQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetSpaceById")
        .WithSummary("Get space by ID")
        .WithDescription("Retrieves a single space by its ID");

        // Get spaces by workspace
        group.MapGet("/", async ([FromQuery] Guid workspaceId, ISender sender) =>
        {
            var query = new GetSpacesByWorkspaceQuery(workspaceId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetSpacesByWorkspace")
        .WithSummary("Get spaces by workspace")
        .WithDescription("Retrieves all spaces in the specified workspace");

        // Update space
        group.MapPut("/{id}", async (
            Guid id,
            UpdateSpaceRequest request,
            ISender sender) =>
        {
            var command = new UpdateSpaceCommand(
                id,
                request.Name,
                request.Description,
                request.Color,
                request.Icon,
                request.IsPrivate
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateSpace")
        .WithSummary("Update space")
        .WithDescription("Updates an existing space");

        // Delete space
        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteSpaceCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteSpace")
        .WithSummary("Delete space")
        .WithDescription("Deletes a space (cascades to folders, tasklists, and tasks)");

        // Nested: Get folders by space
        group.MapGet("/{id}/folders", async (Guid id, ISender sender) =>
        {
            var query = new GetFoldersBySpaceQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetFoldersBySpace")
        .WithSummary("Get folders by space")
        .WithDescription("Retrieves all folders in the specified space");

        // Nested: Create folder in space
        group.MapPost("/{id}/folders", async (
            Guid id,
            CreateFolderRequest request,
            ISender sender) =>
        {
            var command = new CreateFolderCommand(
                id, // Use space ID from route
                request.Name,
                request.Description,
                request.Color,
                request.Icon
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/folders/{result.Value.Id}", result.Value);
        })
        .WithName("CreateFolderInSpace")
        .WithSummary("Create folder in space")
        .WithDescription("Creates a new folder in the specified space");

        // Nested: Create tasklist in space
        group.MapPost("/{id}/lists", async (
            Guid id,
            CreateTaskListRequest request,
            ISender sender) =>
        {
            var command = new CreateTaskListCommand(
                id, // Use space ID from route
                null, // No folder when creating directly under space
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
        .WithName("CreateTaskListInSpace")
        .WithSummary("Create tasklist in space")
        .WithDescription("Creates a new tasklist directly in the specified space (not in a folder)");
    }
}
