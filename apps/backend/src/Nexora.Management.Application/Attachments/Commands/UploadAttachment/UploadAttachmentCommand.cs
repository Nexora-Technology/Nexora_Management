using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Attachments.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Infrastructure.Services;

namespace Nexora.Management.Application.Attachments.Commands.UploadAttachment;

public record UploadAttachmentCommand(
    Guid TaskId,
    string FileName,
    long FileSizeBytes,
    string? MimeType,
    Stream FileContent
) : IRequest<Result<AttachmentDto>>;

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, Result<AttachmentDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;
    private readonly IFileStorageService _fileStorageService;

    public UploadAttachmentCommandHandler(
        IAppDbContext db,
        IUserContext userContext,
        IFileStorageService fileStorageService)
    {
        _db = db;
        _userContext = userContext;
        _fileStorageService = fileStorageService;
    }

    public async System.Threading.Tasks.Task<Result<AttachmentDto>> Handle(UploadAttachmentCommand request, CancellationToken ct)
    {
        // Validate task exists
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId, ct);
        if (task == null)
        {
            return Result<AttachmentDto>.Failure("Task not found");
        }

        // Upload file
        var filePath = await _fileStorageService.UploadFileAsync(request.FileContent, request.FileName, ct);

        var attachment = new Attachment
        {
            TaskId = request.TaskId,
            UserId = _userContext.UserId,
            FileName = request.FileName,
            FilePath = filePath,
            FileSizeBytes = request.FileSizeBytes,
            MimeType = request.MimeType
        };

        _db.Attachments.Add(attachment);
        await _db.SaveChangesAsync(ct);

        // Get user info for response
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == attachment.UserId, ct);

        var attachmentDto = new AttachmentDto(
            attachment.Id,
            attachment.TaskId,
            attachment.UserId,
            user?.Name ?? string.Empty,
            attachment.FileName,
            attachment.FileSizeBytes,
            attachment.MimeType,
            attachment.CreatedAt
        );

        return Result<AttachmentDto>.Success(attachmentDto);
    }
}
