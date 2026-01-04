using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Queries;

public record SearchPagesQuery(
    Guid WorkspaceId,
    string? SearchTerm,
    string? Status,
    bool? FavoriteOnly,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<(List<PageDto> Pages, int TotalCount)>>;

public class SearchPagesQueryHandler : IRequestHandler<SearchPagesQuery, Result<(List<PageDto> Pages, int TotalCount)>>
{
    private readonly IAppDbContext _db;

    public SearchPagesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<(List<PageDto> Pages, int TotalCount)>> Handle(SearchPagesQuery request, CancellationToken ct)
    {
        var query = _db.Pages
            .Where(p => p.WorkspaceId == request.WorkspaceId);

        // Apply status filter
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(p => p.Status == request.Status);
        }
        else
        {
            // By default, exclude deleted pages
            query = query.Where(p => p.Status != "deleted");
        }

        // Apply favorite filter
        if (request.FavoriteOnly.HasValue && request.FavoriteOnly.Value)
        {
            query = query.Where(p => p.IsFavorite);
        }

        // Apply search term
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(searchTerm) ||
                p.Slug.ToLower().Contains(searchTerm)
            );
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(ct);

        // Apply pagination and ordering
        var pages = await query
            .OrderBy(p => p.PositionOrder)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PageDto(
                p.Id,
                p.WorkspaceId,
                p.ParentPageId,
                p.Title,
                p.Slug,
                p.Icon,
                p.CoverImage,
                p.ContentType,
                p.Status,
                p.IsFavorite,
                p.PositionOrder,
                p.CreatedBy,
                p.UpdatedBy,
                p.CreatedAt,
                p.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<(List<PageDto> Pages, int TotalCount)>.Success((pages, totalCount));
    }
}
