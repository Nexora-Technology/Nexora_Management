using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Commands.CreatePage;

public record CreatePageCommand(
    Guid WorkspaceId,
    string Title,
    Guid? ParentPageId,
    string? Icon,
    string? ContentType
) : IRequest<Result<PageDto>>;

public class CreatePageCommandHandler : IRequestHandler<CreatePageCommand, Result<PageDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreatePageCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<PageDto>> Handle(CreatePageCommand request, CancellationToken ct)
    {
        // Validate workspace exists
        var workspace = await _db.Workspaces.FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, ct);
        if (workspace == null)
        {
            return Result<PageDto>.Failure("Workspace not found");
        }

        // Validate parent page if provided
        if (request.ParentPageId.HasValue)
        {
            var parentPage = await _db.Pages.FirstOrDefaultAsync(p => p.Id == request.ParentPageId.Value, ct);
            if (parentPage == null || parentPage.WorkspaceId != request.WorkspaceId)
            {
                return Result<PageDto>.Failure("Parent page not found or belongs to different workspace");
            }
        }

        // Generate slug from title
        var slug = GenerateSlug(request.Title);

        // Ensure slug is unique within workspace
        var slugExists = await _db.Pages
            .AnyAsync(p => p.WorkspaceId == request.WorkspaceId && p.Slug == slug, ct);

        if (slugExists)
        {
            var counter = 1;
            do
            {
                slug = $"{GenerateSlug(request.Title)}-{counter}";
                counter++;
            } while (await _db.Pages.AnyAsync(p => p.WorkspaceId == request.WorkspaceId && p.Slug == slug, ct));
        }

        // Get max position for ordering
        var maxPosition = await _db.Pages
            .Where(p => p.WorkspaceId == request.WorkspaceId && p.ParentPageId == request.ParentPageId)
            .MaxAsync(p => (int?)p.PositionOrder) ?? 0;

        var page = new Page
        {
            WorkspaceId = request.WorkspaceId,
            ParentPageId = request.ParentPageId,
            Title = request.Title,
            Slug = slug,
            Icon = request.Icon,
            ContentType = request.ContentType ?? "rich-text",
            Content = System.Text.Json.JsonDocument.Parse("{}"),
            Status = "active",
            IsFavorite = false,
            PositionOrder = maxPosition + 1,
            CreatedBy = _userContext.UserId,
            UpdatedBy = _userContext.UserId
        };

        _db.Pages.Add(page);
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

    private static string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Trim()
            .Replace(" ", "-")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("?", "")
            .Replace("!", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace(";", "")
            .Replace(":", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Substring(0, Math.Min(100, title.Length));
    }
}
