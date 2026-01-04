using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Documents.Commands.CreatePage;
using Nexora.Management.Application.Documents.Commands.DeletePage;
using Nexora.Management.Application.Documents.Commands.MovePage;
using Nexora.Management.Application.Documents.Commands.RestorePageVersion;
using Nexora.Management.Application.Documents.Commands.ToggleFavorite;
using Nexora.Management.Application.Documents.Commands.UpdatePage;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Application.Documents.Queries;
using System.Text.Json;

namespace Nexora.Management.API.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documents")
            .WithTags("Documents")
            .WithOpenApi()
            .RequireAuthorization();

        // Create page
        group.MapPost("/", async (
            CreatePageRequest request,
            ISender sender) =>
        {
            var command = new CreatePageCommand(
                request.WorkspaceId,
                request.Title,
                request.ParentPageId,
                request.Icon,
                request.ContentType
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/documents/{result.Value.Id}", result.Value);
        })
        .WithName("CreatePage")
        .WithSummary("Create a new page");

        // Get page by ID
        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetPageByIdQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetPageById")
        .WithSummary("Get page by ID");

        // Update page
        group.MapPut("/{id}", async (
            Guid id,
            UpdatePageRequest request,
            ISender sender) =>
        {
            var command = new UpdatePageCommand(
                id,
                request.Title,
                request.Content,
                request.Icon,
                request.CoverImage
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdatePage")
        .WithSummary("Update page");

        // Delete page (soft delete)
        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeletePageCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeletePage")
        .WithSummary("Delete page (soft delete)");

        // Get page tree for workspace
        group.MapGet("/tree/{workspaceId}", async (Guid workspaceId, ISender sender) =>
        {
            var query = new GetPageTreeQuery(workspaceId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetPageTree")
        .WithSummary("Get hierarchical page tree for workspace");

        // Toggle favorite
        group.MapPost("/{id}/favorite", async (Guid id, ISender sender) =>
        {
            var command = new ToggleFavoriteCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(new { isFavorite = result.Value });
        })
        .WithName("TogglePageFavorite")
        .WithSummary("Toggle page favorite status");

        // Get page version history
        group.MapGet("/{id}/versions", async (Guid id, ISender sender) =>
        {
            var query = new GetPageHistoryQuery(id);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetPageHistory")
        .WithSummary("Get page version history");

        // Restore page version
        group.MapPost("/{id}/restore", async (
            Guid id,
            RestoreVersionRequest request,
            ISender sender) =>
        {
            var command = new RestorePageVersionCommand(id, request.VersionNumber);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("RestorePageVersion")
        .WithSummary("Restore page to specific version");

        // Move page
        group.MapPost("/{id}/move", async (
            Guid id,
            MovePageRequest request,
            ISender sender) =>
        {
            var command = new MovePageCommand(
                id,
                request.NewParentPageId,
                request.NewPositionOrder
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("MovePage")
        .WithSummary("Move page to new parent/position");

        // Search pages
        group.MapGet("/search", async (
            [AsParameters] SearchPagesRequest request,
            ISender sender) =>
        {
            var query = new SearchPagesQuery(
                request.WorkspaceId,
                request.SearchTerm,
                request.Status,
                request.FavoriteOnly,
                request.Page,
                request.PageSize
            );
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(new
            {
                pages = result.Value.Pages,
                totalCount = result.Value.TotalCount,
                page = request.Page,
                pageSize = request.PageSize
            });
        })
        .WithName("SearchPages")
        .WithSummary("Search pages with filters");
    }
}
