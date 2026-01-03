using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Attachments.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Attachments.Queries.GetAttachments;

public record GetAttachmentsQuery(Guid TaskId) : IRequest<Result<List<AttachmentDto>>>;

public class GetAttachmentsQueryHandler : IRequestHandler<GetAttachmentsQuery, Result<List<AttachmentDto>>>
{
    private readonly IAppDbContext _db;

    public GetAttachmentsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<AttachmentDto>>> Handle(GetAttachmentsQuery request, CancellationToken ct)
    {
        // Validate task exists
        var taskExists = await _db.Tasks.AnyAsync(t => t.Id == request.TaskId, ct);
        if (!taskExists)
        {
            return Result<List<AttachmentDto>>.Failure("Task not found");
        }

        var attachments = await _db.Attachments
            .Where(a => a.TaskId == request.TaskId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AttachmentDto(
                a.Id,
                a.TaskId,
                a.UserId,
                a.User.Name,
                a.FileName,
                a.FileSizeBytes,
                a.MimeType,
                a.CreatedAt
            ))
            .ToListAsync(ct);

        return Result<List<AttachmentDto>>.Success(attachments);
    }
}
