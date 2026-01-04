using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Commands.MovePage;

public record MovePageCommand(
    Guid PageId,
    Guid? NewParentPageId,
    int NewPositionOrder
) : IRequest<Result<PageDto>>;

public class MovePageCommandHandler : IRequestHandler<MovePageCommand, Result<PageDto>>
{
    private readonly IAppDbContext _db;

    public MovePageCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<PageDto>> Handle(MovePageCommand request, CancellationToken ct)
    {
        var page = await _db.Pages.FirstOrDefaultAsync(p => p.Id == request.PageId, ct);

        if (page == null)
        {
            return Result<PageDto>.Failure("Page not found");
        }

        // Validate new parent page if provided
        if (request.NewParentPageId.HasValue)
        {
            var newParent = await _db.Pages.FirstOrDefaultAsync(p => p.Id == request.NewParentPageId.Value, ct);
            if (newParent == null)
            {
                return Result<PageDto>.Failure("New parent page not found");
            }

            // Ensure new parent is in same workspace
            if (newParent.WorkspaceId != page.WorkspaceId)
            {
                return Result<PageDto>.Failure("Cannot move page to a different workspace");
            }

            // Prevent circular reference (page cannot be parent of itself)
            if (request.NewParentPageId.Value == request.PageId)
            {
                return Result<PageDto>.Failure("Page cannot be its own parent");
            }
        }

        // Update parent and position
        page.ParentPageId = request.NewParentPageId;
        page.PositionOrder = request.NewPositionOrder;

        await _db.SaveChangesAsync(ct);

        var pageDto = new PageDto(
            page.Id,
            page.WorkspaceId,
            page.ParentPageId,
            page.Title,
            page.Slug,
            page.Icon,
            page.CoverImage,
            page.ContentType,
            page.Status,
            page.IsFavorite,
            page.PositionOrder,
            page.CreatedBy,
            page.UpdatedBy,
            page.CreatedAt,
            page.UpdatedAt
        );

        return Result<PageDto>.Success(pageDto);
    }
}
