using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Documents.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Commands.ToggleFavorite;

public record ToggleFavoriteCommand(Guid PageId) : IRequest<Result<bool>>;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public ToggleFavoriteCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<bool>> Handle(ToggleFavoriteCommand request, CancellationToken ct)
    {
        var page = await _db.Pages.FirstOrDefaultAsync(p => p.Id == request.PageId, ct);

        if (page == null)
        {
            return Result<bool>.Failure("Page not found");
        }

        // Toggle favorite flag
        page.IsFavorite = !page.IsFavorite;

        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(page.IsFavorite);
    }
}
