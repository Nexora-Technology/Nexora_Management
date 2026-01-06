using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Folders.Commands.DeleteFolder;

public record DeleteFolderCommand(Guid Id) : IRequest<Result>;

public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeleteFolderCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteFolderCommand request, CancellationToken ct)
    {
        var folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == request.Id, ct);
        if (folder == null)
        {
            return Result.Failure("Folder not found");
        }

        _db.Folders.Remove(folder);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
