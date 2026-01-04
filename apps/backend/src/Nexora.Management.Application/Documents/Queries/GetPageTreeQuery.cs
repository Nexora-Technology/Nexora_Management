using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Queries;

public record GetPageTreeQuery(Guid WorkspaceId) : IRequest<Result<List<PageTreeNodeDto>>>;

public class GetPageTreeQueryHandler : IRequestHandler<GetPageTreeQuery, Result<List<PageTreeNodeDto>>>
{
    private readonly IAppDbContext _db;

    public GetPageTreeQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<PageTreeNodeDto>>> Handle(GetPageTreeQuery request, CancellationToken ct)
    {
        // Get all active pages for the workspace
        var pages = await _db.Pages
            .Where(p => p.WorkspaceId == request.WorkspaceId && p.Status == "active")
            .OrderBy(p => p.PositionOrder)
            .ToListAsync(ct);

        if (pages.Count == 0)
        {
            return Result<List<PageTreeNodeDto>>.Success(new List<PageTreeNodeDto>());
        }

        // Build hierarchical tree structure
        var pageDict = pages.ToDictionary(p => p.Id, p => new PageTreeNodeDto
        {
            Id = p.Id,
            ParentPageId = p.ParentPageId,
            Title = p.Title,
            Slug = p.Slug,
            Icon = p.Icon,
            ContentType = p.ContentType,
            Status = p.Status,
            IsFavorite = p.IsFavorite,
            PositionOrder = p.PositionOrder,
            Children = new List<PageTreeNodeDto>()
        });

        // Build tree by assigning children to parents
        var rootPages = new List<PageTreeNodeDto>();

        foreach (var page in pages)
        {
            var node = pageDict[page.Id];

            if (page.ParentPageId.HasValue && pageDict.ContainsKey(page.ParentPageId.Value))
            {
                pageDict[page.ParentPageId.Value].Children.Add(node);
            }
            else
            {
                rootPages.Add(node);
            }
        }

        return Result<List<PageTreeNodeDto>>.Success(rootPages);
    }
}
