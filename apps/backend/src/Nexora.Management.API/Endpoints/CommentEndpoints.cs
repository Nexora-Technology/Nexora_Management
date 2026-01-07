using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using Nexora.Management.Application.Authorization;
using Nexora.Management.API.Extensions;
using Nexora.Management.Application.Comments.Commands.AddComment;
using Nexora.Management.Application.Comments.Commands.DeleteComment;
using Nexora.Management.Application.Comments.Commands.UpdateComment;
using Nexora.Management.Application.Comments.Queries.GetCommentReplies;
using Nexora.Management.Application.Comments.Queries.GetComments;
using Nexora.Management.API.Hubs;
using Nexora.Management.Application.DTOs.SignalR;
using System.Security.Claims;

namespace Nexora.Management.API.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/comments")
            .RequireAuthorization();

        // Add comment to task
        group.MapPost("", async (
            [FromBody] AddCommentCommand command,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            // Broadcast CommentAdded to task's project group
            var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new CommentUpdatedMessage
            {
                CommentId = result.Value.Id,
                TaskId = result.Value.TaskId,
                Type = "added",
                UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                Timestamp = DateTime.UtcNow,
                Data = result.Value
            };

            // Get task's tasklist ID from database
            var task = await sender.Send(new Nexora.Management.Application.Tasks.Queries.GetTaskByIdQuery(result.Value.TaskId));
            if (task.IsSuccess)
            {
                await taskHub.Clients.Group($"tasklist_{task.Value.TaskListId}")
                    .SendAsync("CommentAdded", message);
            }

            return Results.Ok(result.Value);
        })
        .WithName("AddComment")
        .WithOpenApi()
        .RequirePermission("tasks", "comment");

        // Get comments for task
        group.MapGet("/task/{taskId:guid}", async (Guid taskId, ISender sender) =>
        {
            var query = new GetCommentsQuery(taskId);
            var result = await sender.Send(query);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        })
        .WithName("GetComments")
        .WithOpenApi()
        .RequirePermission("tasks", "view");

        // Get replies for comment
        group.MapGet("/{commentId:guid}/replies", async (Guid commentId, ISender sender) =>
        {
            var query = new GetCommentRepliesQuery(commentId);
            var result = await sender.Send(query);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        })
        .WithName("GetCommentReplies")
        .WithOpenApi()
        .RequirePermission("tasks", "view");

        // Update comment
        group.MapPut("/{commentId:guid}", async (
            Guid commentId,
            [FromBody] UpdateCommentCommand command,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            if (commentId != command.Id)
            {
                return Results.BadRequest("Comment ID mismatch");
            }

            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            // Broadcast CommentUpdated to task's project group
            var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new CommentUpdatedMessage
            {
                CommentId = result.Value.Id,
                TaskId = result.Value.TaskId,
                Type = "updated",
                UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                Timestamp = DateTime.UtcNow,
                Data = result.Value
            };

            // Get task's tasklist ID from database
            var task = await sender.Send(new Nexora.Management.Application.Tasks.Queries.GetTaskByIdQuery(result.Value.TaskId));
            if (task.IsSuccess)
            {
                await taskHub.Clients.Group($"tasklist_{task.Value.TaskListId}")
                    .SendAsync("CommentUpdated", message);
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateComment")
        .WithOpenApi()
        .RequirePermission("tasks", "comment");

        // Delete comment
        group.MapDelete("/{commentId:guid}", async (
            Guid commentId,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var command = new DeleteCommentCommand(commentId);
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            // Broadcast CommentDeleted to task's project group
            if (result.Value.HasValue)
            {
                var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var message = new CommentUpdatedMessage
                {
                    CommentId = commentId,
                    TaskId = result.Value.Value,
                    Type = "deleted",
                    UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                    Timestamp = DateTime.UtcNow,
                    Data = null
                };

                // Get task's tasklist ID from database
                var task = await sender.Send(new Nexora.Management.Application.Tasks.Queries.GetTaskByIdQuery(result.Value.Value));
                if (task.IsSuccess)
                {
                    await taskHub.Clients.Group($"tasklist_{task.Value.TaskListId}")
                        .SendAsync("CommentDeleted", message);
                }
            }

            return Results.NoContent();
        })
        .WithName("DeleteComment")
        .WithOpenApi()
        .RequirePermission("tasks", "comment");
    }
}
