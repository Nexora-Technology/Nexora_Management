using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Workspaces.Commands.CreateWorkspace;
using Nexora.Management.Application.Workspaces.Commands.DeleteWorkspace;
using Nexora.Management.Application.Workspaces.Commands.UpdateWorkspace;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Workspaces.Queries.GetWorkspaceById;
using Nexora.Management.Application.Workspaces.Queries.GetWorkspaces;

namespace Nexora.Management.API.Endpoints;

public static class WorkspaceEndpoints
{
    public static void MapWorkspaceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workspaces")
            .WithTags("Workspaces")
            .WithOpenApi();

        // Create workspace
        group.MapPost("/", async (
            CreateWorkspaceRequest request,
            ISender sender) =>
        {
            var command = new CreateWorkspaceCommand(
                request.Name,
                Guid.Empty, // OwnerId should come from UserContext
                request.SettingsJsonb
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/workspaces/{result.Value.Id}", result.Value);
        })
        .WithName("CreateWorkspace")
        .WithSummary("Create a new workspace")
        .WithDescription("Creates a new workspace with the specified name and settings");

        // Get workspace by ID
        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetWorkspaceByIdQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetWorkspaceById")
        .WithSummary("Get workspace by ID")
        .WithDescription("Retrieves a single workspace by its ID");

        // Get workspaces
        group.MapGet("/", async ([FromQuery] Guid? userId, ISender sender) =>
        {
            var query = new GetWorkspacesQuery(userId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetWorkspaces")
        .WithSummary("Get workspaces")
        .WithDescription("Retrieves all workspaces, optionally filtered by user ID");

        // Update workspace
        group.MapPut("/{id}", async (
            Guid id,
            UpdateWorkspaceRequest request,
            ISender sender) =>
        {
            var command = new UpdateWorkspaceCommand(
                id,
                request.Name,
                request.SettingsJsonb
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateWorkspace")
        .WithSummary("Update workspace")
        .WithDescription("Updates an existing workspace");

        // Delete workspace
        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteWorkspaceCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteWorkspace")
        .WithSummary("Delete workspace")
        .WithDescription("Deletes a workspace (cascades to spaces, folders, tasklists, tasks)");
    }
}
