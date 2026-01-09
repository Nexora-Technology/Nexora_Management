using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Analytics.Queries.GetDashboardStats;
using Nexora.Management.Application.Analytics.Queries.GetProjectProgress;
using Nexora.Management.Application.Analytics.Queries.GetTeamWorkload;
using Nexora.Management.API.Extensions;

namespace Nexora.Management.API.Endpoints;

public static class AnalyticsEndpoints
{
    public static void MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analytics")
            .WithTags("Analytics")
            .WithOpenApi()
            .RequireAuthorization();

        // Get dashboard statistics for a workspace
        group.MapGet("/dashboard/{workspaceId}", async (
            Guid workspaceId,
            ISender sender) =>
        {
            var query = new GetDashboardStatsQuery(workspaceId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetDashboardStats")
        .WithSummary("Get dashboard statistics for a workspace")
        .RequirePermission("analytics", "view");

        // Get project progress for a workspace
        group.MapGet("/project/{workspaceId}/progress", async (
            Guid workspaceId,
            ISender sender) =>
        {
            var query = new GetProjectProgressQuery(workspaceId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetProjectProgress")
        .WithSummary("Get project progress for all projects in workspace")
        .RequirePermission("analytics", "view");

        // Get team workload for a workspace
        group.MapGet("/team/{workspaceId}/workload", async (
            Guid workspaceId,
            ISender sender) =>
        {
            var query = new GetTeamWorkloadQuery(workspaceId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTeamWorkload")
        .WithSummary("Get team workload distribution")
        .RequirePermission("analytics", "view");
    }
}
