using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexora.Management.Application.Authorization;
using Nexora.Management.Application.TimeTracking.Commands.ApproveTimesheet;
using Nexora.Management.Application.TimeTracking.Commands.LogTime;
using Nexora.Management.Application.TimeTracking.Commands.StartTime;
using Nexora.Management.Application.TimeTracking.Commands.StopTime;
using Nexora.Management.Application.TimeTracking.Commands.SubmitTimesheet;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Application.TimeTracking.Queries;
using Nexora.Management.Application.TimeTracking.Queries.GetActiveTimer;
using Nexora.Management.Application.TimeTracking.Queries.GetTimeEntries;
using Nexora.Management.Application.TimeTracking.Queries.GetTimesheet;
using Nexora.Management.Application.TimeTracking.Queries.GetUserTimeReport;

namespace Nexora.Management.API.Endpoints;

public static class TimeEndpoints
{
    public static void MapTimeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/time")
            .WithTags("Time")
            .WithOpenApi()
            .RequireAuthorization();

        // Start timer
        group.MapPost("/timer/start", async (
            StartTimeRequest request,
            ISender sender) =>
        {
            var command = new StartTimeCommand(
                request.TaskId,
                request.Description,
                request.IsBillable,
                request.WorkspaceId
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("StartTimer")
        .WithSummary("Start a new timer");

        // Stop timer
        group.MapPost("/timer/stop", async (
            StopTimeRequest request,
            ISender sender) =>
        {
            var command = new StopTimeCommand(request.Description);
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("StopTimer")
        .WithSummary("Stop the active timer");

        // Get active timer
        group.MapGet("/timer/active", async (ISender sender) =>
        {
            var query = new GetActiveTimerQuery();
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetActiveTimer")
        .WithSummary("Get the currently active timer");

        // Manual time entry
        group.MapPost("/entries", async (
            CreateTimeEntryRequest request,
            ISender sender) =>
        {
            var command = new LogTimeCommand(
                request.TaskId,
                request.StartTime,
                request.EndTime,
                request.DurationMinutes,
                request.Description,
                request.IsBillable,
                request.WorkspaceId
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/time/entries/{result.Value.Id}", result.Value);
        })
        .WithName("LogTime")
        .WithSummary("Create a manual time entry");

        // Get time entries
        group.MapGet("/entries", async (
            [AsParameters] GetTimeEntriesQueryRequest request,
            ISender sender) =>
        {
            var query = new GetTimeEntriesQuery(
                request.TaskId,
                request.StartDate,
                request.EndDate,
                request.Status,
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
        .WithName("GetTimeEntries")
        .WithSummary("Get time entries with filters");

        // Get timesheet
        group.MapGet("/timesheet/{userId}", async (
            Guid userId,
            DateTime weekStart,
            ISender sender) =>
        {
            var query = new GetTimesheetQuery(userId, weekStart);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTimesheet")
        .WithSummary("Get weekly timesheet");

        // Submit timesheet for approval
        group.MapPost("/timesheet/submit", async (
            [FromBody] TimesheetSubmitRequest request,
            ISender sender) =>
        {
            var command = new SubmitTimesheetCommand(
                request.UserId,
                request.WeekStart,
                request.WeekEnd
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(new { message = "Timesheet submitted for approval" });
        })
        .WithName("SubmitTimesheet")
        .WithSummary("Submit timesheet for approval");

        // Approve/reject timesheet (manager only)
        group.MapPost("/timesheet/approve", async (
            [FromBody] TimesheetApprovalRequest request,
            ISender sender) =>
        {
            var command = new ApproveTimesheetCommand(
                request.UserId,
                request.WeekStart,
                request.WeekEnd,
                request.Status
            );
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(new { message = $"Timesheet {request.Status}" });
        })
        .WithName("ApproveTimesheet")
        .WithSummary("Approve or reject timesheet")
        .WithMetadata(new RequirePermissionAttribute("time", "approve"));

        // Get time report
        group.MapGet("/reports", async (
            Guid userId,
            DateTime periodStart,
            DateTime periodEnd,
            ISender sender) =>
        {
            var query = new GetUserTimeReportQuery(userId, periodStart, periodEnd);
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetTimeReport")
        .WithSummary("Get time report for a period");
    }
}
