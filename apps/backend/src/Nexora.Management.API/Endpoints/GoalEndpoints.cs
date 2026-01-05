using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Goals.Commands.CreateObjective;
using Nexora.Management.Application.Goals.Commands.CreateKeyResult;
using Nexora.Management.Application.Goals.Commands.CreatePeriod;
using Nexora.Management.Application.Goals.Commands.DeleteObjective;
using Nexora.Management.Application.Goals.Commands.DeleteKeyResult;
using Nexora.Management.Application.Goals.Commands.DeletePeriod;
using Nexora.Management.Application.Goals.Commands.UpdateObjective;
using Nexora.Management.Application.Goals.Commands.UpdateKeyResult;
using Nexora.Management.Application.Goals.Commands.UpdatePeriod;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Application.Goals.Queries.GetObjectives;
using Nexora.Management.Application.Goals.Queries.GetObjectiveTree;
using Nexora.Management.Application.Goals.Queries.GetProgressDashboard;
using Nexora.Management.Application.Goals.Queries.GetPeriods;

namespace Nexora.Management.API.Endpoints;

public static class GoalEndpoints
{
    public static void MapGoalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/goals")
            .WithTags("Goals")
            .WithOpenApi()
            .RequireAuthorization();

        // ==================== PERIODS ====================

        // Create period
        group.MapPost("/periods", async (
            CreatePeriodRequest request,
            ISender sender) =>
        {
            var command = new CreatePeriodCommand(
                request.WorkspaceId,
                request.Name,
                request.StartDate,
                request.EndDate
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/goals/periods/{result.Value.Id}", result.Value);
        })
        .WithName("CreatePeriod")
        .WithSummary("Create a new goal period");

        // Get periods
        group.MapGet("/periods", async (
            Guid workspaceId,
            string? status,
            ISender sender) =>
        {
            var query = new GetPeriodsQuery(workspaceId, status);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetPeriods")
        .WithSummary("Get all goal periods for a workspace");

        // Update period
        group.MapPut("/periods/{id}", async (
            Guid id,
            UpdatePeriodRequest request,
            ISender sender) =>
        {
            var command = new UpdatePeriodCommand(
                id,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.Status
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdatePeriod")
        .WithSummary("Update a goal period");

        // Delete period
        group.MapDelete("/periods/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var command = new DeletePeriodCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeletePeriod")
        .WithSummary("Delete a goal period");

        // ==================== OBJECTIVES ====================

        // Create objective
        group.MapPost("/objectives", async (
            CreateObjectiveRequest request,
            ISender sender) =>
        {
            var command = new CreateObjectiveCommand(
                request.WorkspaceId,
                request.PeriodId,
                request.ParentObjectiveId,
                request.Title,
                request.Description,
                request.OwnerId,
                request.Weight
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/goals/objectives/{result.Value.Id}", result.Value);
        })
        .WithName("CreateObjective")
        .WithSummary("Create a new objective");

        // Get objectives (paginated)
        group.MapGet("/objectives", async (
            Guid workspaceId,
            Guid? periodId,
            Guid? parentObjectiveId,
            string? status,
            int page,
            int pageSize,
            ISender sender) =>
        {
            var query = new GetObjectivesQuery(workspaceId, periodId, parentObjectiveId, status, page, pageSize);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetObjectives")
        .WithSummary("Get objectives with pagination and filters");

        // Get objective tree (hierarchical)
        group.MapGet("/objectives/tree", async (
            Guid workspaceId,
            Guid? periodId,
            ISender sender) =>
        {
            var query = new GetObjectiveTreeQuery(workspaceId, periodId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetObjectiveTree")
        .WithSummary("Get hierarchical tree of objectives");

        // Update objective
        group.MapPut("/objectives/{id}", async (
            Guid id,
            UpdateObjectiveRequest request,
            ISender sender) =>
        {
            var command = new UpdateObjectiveCommand(
                id,
                request.Title,
                request.Description,
                request.OwnerId,
                request.Weight,
                request.Status,
                request.PositionOrder
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateObjective")
        .WithSummary("Update an objective");

        // Delete objective
        group.MapDelete("/objectives/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var command = new DeleteObjectiveCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteObjective")
        .WithSummary("Delete an objective");

        // ==================== KEY RESULTS ====================

        // Create key result
        group.MapPost("/keyresults", async (
            CreateKeyResultRequest request,
            ISender sender) =>
        {
            var command = new CreateKeyResultCommand(
                request.ObjectiveId,
                request.Title,
                request.MetricType,
                request.CurrentValue,
                request.TargetValue,
                request.Unit,
                request.DueDate,
                request.Weight
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/goals/keyresults/{result.Value.Id}", result.Value);
        })
        .WithName("CreateKeyResult")
        .WithSummary("Create a new key result");

        // Update key result
        group.MapPut("/keyresults/{id}", async (
            Guid id,
            UpdateKeyResultRequest request,
            ISender sender) =>
        {
            var command = new UpdateKeyResultCommand(
                id,
                request.Title,
                request.CurrentValue,
                request.TargetValue,
                request.DueDate,
                request.Weight
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateKeyResult")
        .WithSummary("Update a key result");

        // Delete key result
        group.MapDelete("/keyresults/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var command = new DeleteKeyResultCommand(id);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.NoContent();
        })
        .WithName("DeleteKeyResult")
        .WithSummary("Delete a key result");

        // ==================== DASHBOARD ====================

        // Get progress dashboard
        group.MapGet("/dashboard", async (
            Guid workspaceId,
            Guid? periodId,
            ISender sender) =>
        {
            var query = new GetProgressDashboardQuery(workspaceId, periodId);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetProgressDashboard")
        .WithSummary("Get progress dashboard statistics");
    }
}

// ==================== REQUEST DTOs ====================

public record CreatePeriodRequest(
    Guid WorkspaceId,
    string Name,
    DateTime StartDate,
    DateTime EndDate
);

public record UpdatePeriodRequest(
    string? Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Status
);

public record CreateObjectiveRequest(
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string Title,
    string? Description,
    Guid? OwnerId,
    int Weight = 1
);

public record UpdateObjectiveRequest(
    string? Title,
    string? Description,
    Guid? OwnerId,
    int? Weight,
    string? Status,
    int? PositionOrder
);

public record CreateKeyResultRequest(
    Guid ObjectiveId,
    string Title,
    string MetricType,
    decimal CurrentValue,
    decimal TargetValue,
    string Unit,
    DateTime? DueDate,
    int Weight = 1
);

public record UpdateKeyResultRequest(
    string? Title,
    decimal? CurrentValue,
    decimal? TargetValue,
    DateTime? DueDate,
    int? Weight
);
