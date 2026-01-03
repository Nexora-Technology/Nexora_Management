using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Nexora.Management.API.Extensions;
using Nexora.Management.Application.Attachments.Commands.DeleteAttachment;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Application.Attachments.Commands.UploadAttachment;
using Nexora.Management.Application.Attachments.Queries.GetAttachments;
using Nexora.Management.Infrastructure.Services;
using Nexora.Management.API.Hubs;
using Nexora.Management.Application.DTOs.SignalR;
using System.Security.Claims;

namespace Nexora.Management.API.Endpoints;

public static class AttachmentEndpoints
{
    public static void MapAttachmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attachments")
            .RequireAuthorization();

        // CRITICAL: Allowed file extensions (prevent executable uploads)
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".txt", ".csv", ".json", ".xml",
            ".zip", ".rar", ".7z"
        };

        // CRITICAL: Maximum file size (100MB)
        const long maxFileSize = 100 * 1024 * 1024;

        // Upload attachment
        group.MapPost("/upload/{taskId:guid}", async (
            Guid taskId,
            IFormFile file,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded");
            }

            // CRITICAL FIX: Validate file size
            if (file.Length > maxFileSize)
            {
                return Results.BadRequest($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB");
            }

            // CRITICAL FIX: Validate file extension
            var fileExtension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Results.BadRequest($"File type '{fileExtension}' is not allowed");
            }

            var command = new UploadAttachmentCommand(
                taskId,
                file.FileName,
                file.Length,
                file.ContentType,
                file.OpenReadStream()
            );

            var result = await sender.Send(command, ct);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            // Broadcast AttachmentUploaded to task's project group
            var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new AttachmentUpdatedMessage
            {
                AttachmentId = result.Value.Id,
                TaskId = result.Value.TaskId,
                Type = "uploaded",
                UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                Timestamp = DateTime.UtcNow,
                Data = result.Value
            };

            // Get task's project ID from database
            var task = await sender.Send(new Nexora.Management.Application.Tasks.Queries.GetTaskByIdQuery(result.Value.TaskId));
            if (task.IsSuccess)
            {
                await taskHub.Clients.Group($"project_{task.Value.ProjectId}")
                    .SendAsync("AttachmentUploaded", message);
            }

            return Results.Ok(result.Value);
        })
        .WithName("UploadAttachment")
        .WithOpenApi()
        .RequirePermission("tasks", "edit")
        .DisableAntiforgery();

        // Get attachments for task
        group.MapGet("/task/{taskId:guid}", async (Guid taskId, ISender sender) =>
        {
            var query = new GetAttachmentsQuery(taskId);
            var result = await sender.Send(query);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        })
        .WithName("GetAttachments")
        .WithOpenApi()
        .RequirePermission("tasks", "view");

        // Download attachment
        group.MapGet("/{attachmentId:guid}/download", async (
            Guid attachmentId,
            [FromServices] IFileStorageService fileStorageService,
            [FromServices] IAppDbContext db,
            CancellationToken ct) =>
        {
            var attachment = await db.Attachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId, ct);

            if (attachment == null)
            {
                return Results.NotFound("Attachment not found");
            }

            var (fileStream, contentType) = await fileStorageService.GetFileAsync(attachment.FilePath, ct);

            return Results.File(fileStream, contentType, attachment.FileName);
        })
        .WithName("DownloadAttachment")
        .WithOpenApi()
        .RequirePermission("tasks", "view");

        // Delete attachment
        group.MapDelete("/{attachmentId:guid}", async (
            Guid attachmentId,
            ISender sender,
            IHubContext<TaskHub> taskHub,
            HttpContext httpContext) =>
        {
            var command = new DeleteAttachmentCommand(attachmentId);
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            // Broadcast AttachmentDeleted to task's project group
            if (result.Value.HasValue)
            {
                var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var message = new AttachmentUpdatedMessage
                {
                    AttachmentId = attachmentId,
                    TaskId = result.Value.Value,
                    Type = "deleted",
                    UpdatedBy = Guid.Parse(currentUserId ?? Guid.Empty.ToString()),
                    Timestamp = DateTime.UtcNow,
                    Data = null
                };

                // Get task's project ID from database
                var task = await sender.Send(new Nexora.Management.Application.Tasks.Queries.GetTaskByIdQuery(result.Value.Value));
                if (task.IsSuccess)
                {
                    await taskHub.Clients.Group($"project_{task.Value.ProjectId}")
                        .SendAsync("AttachmentDeleted", message);
                }
            }

            return Results.NoContent();
        })
        .WithName("DeleteAttachment")
        .WithOpenApi()
        .RequirePermission("tasks", "edit");
    }
}
