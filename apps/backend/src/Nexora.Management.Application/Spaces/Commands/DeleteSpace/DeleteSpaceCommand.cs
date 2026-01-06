using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Spaces.Commands.DeleteSpace;

public record DeleteSpaceCommand(Guid Id) : IRequest<Result>;

public class DeleteSpaceCommandHandler : IRequestHandler<DeleteSpaceCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeleteSpaceCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteSpaceCommand request, CancellationToken ct)
    {
        var space = await _db.Spaces.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (space == null)
        {
            return Result.Failure("Space not found");
        }

        _db.Spaces.Remove(space);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
