using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Dashboards.Commands.CreateDashboard;
using Nexora.Management.Application.Dashboards.Commands.DeleteDashboard;
using Nexora.Management.Application.Dashboards.Commands.UpdateDashboard;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.API.Extensions;

namespace Nexora.Management.API.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboards")
            .WithTags("Dashboards")
            .WithOpenApi()
            .RequireAuthorization();

        // Get all dashboards for a workspace
        group.MapGet("/", async (
            Guid workspaceId,
            ISender sender,
            IAppDbContext db) =>
        {
            var dashboards = await db.Dashboards
                .Where(d => d.WorkspaceId == workspaceId)
                .Select(d => new DashboardDto(
                    d.Id,
                    d.WorkspaceId,
                    d.Name,
                    d.Layout,
                    d.CreatedBy,
                    d.IsTemplate,
                    d.CreatedAt,
                    d.UpdatedAt
                ))
                .ToListAsync();

            return Results.Ok(dashboards);
        })
        .WithName("GetDashboards")
        .WithSummary("Get all dashboards for a workspace")
        .RequirePermission("dashboards", "view");

        // Get a specific dashboard
        group.MapGet("/{id}", async (
            Guid id,
            ISender sender,
            IAppDbContext db) =>
        {
            var dashboard = await db.Dashboards
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dashboard == null)
            {
                return Results.NotFound(new { error = "Dashboard not found" });
            }

            var dashboardDto = new DashboardDto(
                dashboard.Id,
                dashboard.WorkspaceId,
                dashboard.Name,
                dashboard.Layout,
                dashboard.CreatedBy,
                dashboard.IsTemplate,
                dashboard.CreatedAt,
                dashboard.UpdatedAt
            );

            return Results.Ok(dashboardDto);
        })
        .WithName("GetDashboard")
        .WithSummary("Get a specific dashboard by ID")
        .RequirePermission("dashboards", "view");

        // Create a new dashboard
        group.MapPost("/", async (
            [FromBody] CreateDashboardRequest request,
            ISender sender) =>
        {
            var command = new CreateDashboardCommand(
                request.WorkspaceId,
                request.Name,
                request.Layout,
                request.IsTemplate
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/dashboards/{result.Value.Id}", result.Value);
        })
        .WithName("CreateDashboard")
        .WithSummary("Create a new dashboard")
        .RequirePermission("dashboards", "create");

        // Update a dashboard
        group.MapPut("/{id}", async (
            Guid id,
            [FromBody] UpdateDashboardRequest request,
            ISender sender) =>
        {
            var command = new UpdateDashboardCommand(
                id,
                request.Name,
                request.Layout
            );

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateDashboard")
        .WithSummary("Update an existing dashboard")
        .RequirePermission("dashboards", "edit");

        // Delete a dashboard
        group.MapDelete("/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var command = new DeleteDashboardCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(new { message = "Dashboard deleted successfully" });
        })
        .WithName("DeleteDashboard")
        .WithSummary("Delete a dashboard")
        .RequirePermission("dashboards", "delete");
    }
}
