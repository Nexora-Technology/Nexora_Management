using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Queries;

public record GetPageByIdQuery(Guid PageId) : IRequest<Result<PageDetailDto>>;

public class GetPageByIdQueryHandler : IRequestHandler<GetPageByIdQuery, Result<PageDetailDto>>
{
    private readonly IAppDbContext _db;

    public GetPageByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<PageDetailDto>> Handle(GetPageByIdQuery request, CancellationToken ct)
    {
        var page = await _db.Pages
            .Include(p => p.Creator)
            .Include(p => p.Updater)
            .FirstOrDefaultAsync(p => p.Id == request.PageId, ct);

        if (page == null)
        {
            return Result<PageDetailDto>.Failure("Page not found");
        }

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
