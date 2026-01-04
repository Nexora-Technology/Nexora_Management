using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Commands.RestorePageVersion;

public record RestorePageVersionCommand(
    Guid PageId,
    int VersionNumber
) : IRequest<Result<PageDetailDto>>;

public class RestorePageVersionCommandHandler : IRequestHandler<RestorePageVersionCommand, Result<PageDetailDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public RestorePageVersionCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<PageDetailDto>> Handle(RestorePageVersionCommand request, CancellationToken ct)
    {
        // Load the page with navigation properties
        var page = await _db.Pages
            .Include(p => p.Creator)
            .Include(p => p.Updater)
            .FirstOrDefaultAsync(p => p.Id == request.PageId, ct);

        if (page == null)
        {
            return Result<PageDetailDto>.Failure("Page not found");
        }

        // Find the version to restore
        var version = await _db.PageVersions
            .FirstOrDefaultAsync(v => v.PageId == request.PageId && v.VersionNumber == request.VersionNumber, ct);

        if (version == null)
        {
            return Result<PageDetailDto>.Failure("Version not found");
        }

        // Create a version snapshot of current state before restoring
        var currentVersionNumber = await _db.PageVersions
            .Where(v => v.PageId == page.Id)
            .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

        var snapshotVersion = new PageVersion
        {
            PageId = page.Id,
            VersionNumber = currentVersionNumber + 1,
            Content = page.Content,
            CommitMessage = $"Auto-save before restore to version {request.VersionNumber} by {_userContext.UserId}",
            CreatedBy = _userContext.UserId
        };

        _db.PageVersions.Add(snapshotVersion);

        // Restore content from the version
        page.Content = version.Content;
        page.UpdatedBy = _userContext.UserId;

        await _db.SaveChangesAsync(ct);

        var pageDto = new PageDetailDto(
            page.Id,
            page.WorkspaceId,
            page.ParentPageId,
            page.Title,
            page.Slug,
            page.Icon,
            page.CoverImage,
            page.Content,
            page.ContentType,
            page.Status,
            page.IsFavorite,
            page.PositionOrder,
            page.CreatedBy,
            page.Creator?.Email,
            page.UpdatedBy,
            page.Updater?.Email,
            page.CreatedAt,
            page.UpdatedAt
        );

        return Result<PageDetailDto>.Success(pageDto);
    }
}
