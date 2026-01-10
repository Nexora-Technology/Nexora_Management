using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Workspaces.Commands.AddWorkspaceMember;
using Nexora.Management.Application.Workspaces.Commands.CreateWorkspace;
using Nexora.Management.Application.Workspaces.Commands.DeleteWorkspace;
using Nexora.Management.Application.Workspaces.Commands.RemoveWorkspaceMember;
using Nexora.Management.Application.Workspaces.Commands.TransferWorkspaceOwnership;
using Nexora.Management.Application.Workspaces.Commands.UpdateWorkspace;
using Nexora.Management.Application.Workspaces.Commands.UpdateWorkspaceMemberRole;
using Nexora.Management.Application.Workspaces.DTOs;
using Nexora.Management.Application.Workspaces.Queries.GetWorkspaceById;
using Nexora.Management.Application.Workspaces.Queries.GetWorkspaceMemberById;
using Nexora.Management.Application.Workspaces.Queries.GetWorkspaceMembers;
using Nexora.Management.Application.Workspaces.Queries.GetUserWorkspaces;
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

        // === Member Management Endpoints ===

        var membersGroup = app.MapGroup("/api/workspaces/{workspaceId}/members")
            .WithTags("Workspace Members")
            .WithOpenApi();

        // Get all members of a workspace
        membersGroup.MapGet("/", async (Guid workspaceId, ISender sender) =>
        {
            var query = new GetWorkspaceMembersQuery(workspaceId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetWorkspaceMembers")
        .WithSummary("Get workspace members")
        .WithDescription("Retrieves all members of a workspace with their roles");

        // Get specific member of a workspace
        membersGroup.MapGet("/{userId}", async (Guid workspaceId, Guid userId, ISender sender) =>
        {
            var query = new GetWorkspaceMemberByIdQuery(workspaceId, userId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetWorkspaceMember")
        .WithSummary("Get workspace member")
        .WithDescription("Retrieves a specific member of a workspace by user ID");

        // Add member to workspace
        membersGroup.MapPost("/", async (
            Guid workspaceId,
            AddWorkspaceMemberRequest request,
            ISender sender) =>
        {
            var command = new AddWorkspaceMemberCommand(workspaceId, request);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/workspaces/{workspaceId}/members/{result.Value.UserId}", result.Value);
        })
        .WithName("AddWorkspaceMember")
        .WithSummary("Add workspace member")
        .WithDescription("Adds a new member to a workspace with the specified role");

        // Update member role
        membersGroup.MapPut("/{userId}", async (
            Guid workspaceId,
            Guid userId,
            UpdateWorkspaceMemberRoleRequest request,
            ISender sender) =>
        {
            var command = new UpdateWorkspaceMemberRoleCommand(workspaceId, userId, request);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateWorkspaceMemberRole")
        .WithSummary("Update member role")
        .WithDescription("Updates the role of a workspace member");

        // Remove member from workspace
        membersGroup.MapDelete("/{userId}", async (Guid workspaceId, Guid userId, ISender sender) =>
        {
            var command = new RemoveWorkspaceMemberCommand(workspaceId, userId);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("RemoveWorkspaceMember")
        .WithSummary("Remove workspace member")
        .WithDescription("Removes a member from a workspace");

        // Transfer workspace ownership
        membersGroup.MapPost("/transfer", async (
            Guid workspaceId,
            TransferWorkspaceOwnershipRequest request,
            ISender sender) =>
        {
            var command = new TransferWorkspaceOwnershipCommand(
                workspaceId,
                Guid.Empty, // CurrentOwnerId should come from UserContext
                request
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("TransferWorkspaceOwnership")
        .WithSummary("Transfer workspace ownership")
        .WithDescription("Transfers ownership of a workspace to another member");

        // Get user's workspaces
        app.MapGet("/api/users/{userId}/workspaces", async (Guid userId, ISender sender) =>
        {
            var query = new GetUserWorkspacesQuery(userId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithTags("Workspaces")
        .WithName("GetUserWorkspaces")
        .WithSummary("Get user workspaces")
        .WithDescription("Retrieves all workspaces where the user is a member or owner");
    }
}
