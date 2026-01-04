using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Queries;

public record GetPageHistoryQuery(Guid PageId) : IRequest<Result<List<PageVersionDto>>>;

public class GetPageHistoryQueryHandler : IRequestHandler<GetPageHistoryQuery, Result<List<PageVersionDto>>>
{
    private readonly IAppDbContext _db;

    public GetPageHistoryQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<PageVersionDto>>> Handle(GetPageHistoryQuery request, CancellationToken ct)
    {
        // Verify page exists
        var pageExists = await _db.Pages.AnyAsync(p => p.Id == request.PageId, ct);
        if (!pageExists)
        {
            return Result<List<PageVersionDto>>.Failure("Page not found");
        }

        // Get version history with creator info
        var versions = await _db.PageVersions
            .Include(v => v.Creator)
            .Where(v => v.PageId == request.PageId)
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => new PageVersionDto(
                v.Id,
                v.PageId,
                v.VersionNumber,
                v.Content,
                v.CommitMessage,
                v.CreatedBy,
                v.Creator != null ? v.Creator.Email : null,
                v.CreatedAt
            ))
            .ToListAsync(ct);

        return Result<List<PageVersionDto>>.Success(versions);
    }
}
