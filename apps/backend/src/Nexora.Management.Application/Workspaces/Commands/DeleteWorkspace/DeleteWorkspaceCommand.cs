using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Workspaces.Commands.DeleteWorkspace;

public record DeleteWorkspaceCommand(Guid Id) : IRequest<Result>;

public class DeleteWorkspaceCommandHandler : IRequestHandler<DeleteWorkspaceCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeleteWorkspaceCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteWorkspaceCommand request, CancellationToken ct)
    {
        var workspace = await _db.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.Id, ct);

        if (workspace == null)
        {
            return Result.Failure("Workspace not found");
        }

        _db.Workspaces.Remove(workspace);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
