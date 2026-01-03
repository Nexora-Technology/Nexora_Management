using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.Infrastructure.Services;

namespace Nexora.Management.Application.Attachments.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(Guid Id) : IRequest<Result>;

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;
    private readonly IFileStorageService _fileStorageService;

    public DeleteAttachmentCommandHandler(
        IAppDbContext db,
        IUserContext userContext,
        IFileStorageService fileStorageService)
    {
        _db = db;
        _userContext = userContext;
        _fileStorageService = fileStorageService;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken ct)
    {
        var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (attachment == null)
        {
            return Result.Failure("Attachment not found");
        }

        // Only the uploader can delete the attachment
        if (attachment.UserId != _userContext.UserId)
        {
            return Result.Failure("You can only delete your own attachments");
        }

        // Delete file from storage
        await _fileStorageService.DeleteFileAsync(attachment.FilePath, ct);

        // Delete database record
        _db.Attachments.Remove(attachment);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
