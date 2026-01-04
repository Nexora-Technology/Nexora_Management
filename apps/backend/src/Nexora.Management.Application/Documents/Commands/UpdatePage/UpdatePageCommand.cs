using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Commands.UpdatePage;

public record UpdatePageCommand(
    Guid PageId,
    string Title,
    System.Text.Json.JsonDocument Content,
    string? Icon,
    string? CoverImage
) : IRequest<Result<PageDetailDto>>;

public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, Result<PageDetailDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public UpdatePageCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<PageDetailDto>> Handle(UpdatePageCommand request, CancellationToken ct)
    {
        var page = await _db.Pages
            .Include(p => p.Creator)
            .Include(p => p.Updater)
            .FirstOrDefaultAsync(p => p.Id == request.PageId, ct);

        if (page == null)
        {
            return Result<PageDetailDto>.Failure("Page not found");
        }

        // Create version snapshot before update
        var currentVersionNumber = await _db.PageVersions
            .Where(v => v.PageId == page.Id)
            .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

        var version = new PageVersion
        {
            PageId = page.Id,
            VersionNumber = currentVersionNumber + 1,
            Content = page.Content,
            CommitMessage = $"Auto-save before edit by {_userContext.UserId}",
            CreatedBy = _userContext.UserId
        };

        _db.PageVersions.Add(version);

        // Update page
        page.Title = request.Title;
        page.Content = request.Content;
        page.Icon = request.Icon;
        page.CoverImage = request.CoverImage;
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
