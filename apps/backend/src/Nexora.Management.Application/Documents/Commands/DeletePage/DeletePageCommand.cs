using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Documents.Commands.DeletePage;

public record DeletePageCommand(Guid PageId) : IRequest<Result>;

public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeletePageCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeletePageCommand request, CancellationToken ct)
    {
        var page = await _db.Pages.FirstOrDefaultAsync(p => p.Id == request.PageId, ct);

        if (page == null)
        {
            return Result.Failure("Page not found");
        }

        // Soft delete by setting status to "deleted"
        page.Status = "deleted";

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
