using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.DeleteObjective;

public record DeleteObjectiveCommand(Guid ObjectiveId) : IRequest<Result>;

public class DeleteObjectiveCommandHandler : IRequestHandler<DeleteObjectiveCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public DeleteObjectiveCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteObjectiveCommand request, CancellationToken ct)
    {
        var objective = await _db.Objectives
            .Include(o => o.SubObjectives)
            .Include(o => o.KeyResults)
            .FirstOrDefaultAsync(o => o.Id == request.ObjectiveId, ct);

        if (objective == null)
        {
            return Result.Failure("Objective not found");
        }

        // Check if objective has sub-objectives
        if (objective.SubObjectives.Any())
        {
            return Result.Failure("Cannot delete objective with sub-objectives. Delete sub-objectives first.");
        }

        // Delete associated key results (cascade delete should handle this, but explicit for clarity)
        _db.KeyResults.RemoveRange(objective.KeyResults);

        // Delete objective
        _db.Objectives.Remove(objective);

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
