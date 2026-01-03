using Microsoft.AspNetCore.Mvc;
using MediatR;
using Nexora.Management.Application.Authorization;
using Nexora.Management.API.Extensions;
using Nexora.Management.Application.Comments.Commands.AddComment;
using Nexora.Management.Application.Comments.Commands.DeleteComment;
using Nexora.Management.Application.Comments.Commands.UpdateComment;
using Nexora.Management.Application.Comments.Queries.GetCommentReplies;
using Nexora.Management.Application.Comments.Queries.GetComments;

namespace Nexora.Management.API.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/comments")
            .RequireAuthorization();

        // Add comment to task
        group.MapPost("", async ([FromBody] AddCommentCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
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
        group.MapPut("/{commentId:guid}", async (Guid commentId, [FromBody] UpdateCommentCommand command, ISender sender) =>
        {
            if (commentId != command.Id)
            {
                return Results.BadRequest("Comment ID mismatch");
            }

            var result = await sender.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithName("UpdateComment")
        .WithOpenApi()
        .RequirePermission("tasks", "comment");

        // Delete comment
        group.MapDelete("/{commentId:guid}", async (Guid commentId, ISender sender) =>
        {
            var command = new DeleteCommentCommand(commentId);
            var result = await sender.Send(command);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
        })
        .WithName("DeleteComment")
        .WithOpenApi()
        .RequirePermission("tasks", "comment");
    }
}
