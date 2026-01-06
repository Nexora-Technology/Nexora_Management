using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Folders.Commands.CreateFolder;
using Nexora.Management.Application.Folders.Commands.DeleteFolder;
using Nexora.Management.Application.Folders.Commands.UpdateFolder;
using Nexora.Management.Application.Folders.Commands.UpdateFolderPosition;
using Nexora.Management.Application.Folders.DTOs;
using Nexora.Management.Application.Folders.Queries.GetFolderById;
using Nexora.Management.Application.Folders.Queries.GetFoldersBySpace;

namespace Nexora.Management.API.Endpoints;

public static class FolderEndpoints
{
    public static void MapFolderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/folders")
            .WithTags("Folders")
            .WithOpenApi();

        // Create folder
        group.MapPost("/", async (
            CreateFolderRequest request,
            ISender sender) =>
        {
            var command = new CreateFolderCommand(
                request.SpaceId,
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
        .WithName("CreateFolder")
        .WithSummary("Create a new folder")
        .WithDescription("Creates a new folder in the specified space");

        // Get folder by ID
        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetFolderByIdQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetFolderById")
        .WithSummary("Get folder by ID")
        .WithDescription("Retrieves a single folder by its ID");

        // Update folder
        group.MapPut("/{id}", async (
            Guid id,
            UpdateFolderRequest request,
            ISender sender) =>
        {
            var command = new UpdateFolderCommand(
                id,
                request.Name,
                request.Description,
                request.Color,
                request.Icon
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateFolder")
        .WithSummary("Update folder")
        .WithDescription("Updates an existing folder");

        // Update folder position
        group.MapPatch("/{id}/position", async (
            Guid id,
            UpdateFolderPositionRequest request,
            ISender sender) =>
        {
            var command = new UpdateFolderPositionCommand(id, request.PositionOrder);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateFolderPosition")
        .WithSummary("Update folder position")
        .WithDescription("Updates the position order of a folder");

        // Delete folder
        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteFolderCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteFolder")
        .WithSummary("Delete folder")
        .WithDescription("Deletes a folder (cascades to tasklists and tasks)");
    }
}
