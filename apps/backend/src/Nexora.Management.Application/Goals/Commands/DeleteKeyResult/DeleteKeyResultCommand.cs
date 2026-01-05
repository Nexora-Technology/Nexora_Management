using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.DeleteKeyResult;

public record DeleteKeyResultCommand(Guid KeyResultId) : IRequest<Result>;

public class DeleteKeyResultCommandHandler : IRequestHandler<DeleteKeyResultCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public DeleteKeyResultCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteKeyResultCommand request, CancellationToken ct)
    {
        var keyResult = await _db.KeyResults.FirstOrDefaultAsync(kr => kr.Id == request.KeyResultId, ct);

        if (keyResult == null)
        {
            return Result.Failure("Key result not found");
        }

        var objectiveId = keyResult.ObjectiveId;

        _db.KeyResults.Remove(keyResult);
        await _db.SaveChangesAsync(ct);

        // Update objective progress after key result deletion
        await UpdateObjectiveProgressAsync(objectiveId, ct);

        return Result.Success();
    }

    private async System.Threading.Tasks.Task UpdateObjectiveProgressAsync(Guid objectiveId, CancellationToken ct)
    {
        var keyResults = await _db.KeyResults
            .Where(kr => kr.ObjectiveId == objectiveId)
            .ToListAsync(ct);

        if (!keyResults.Any())
        {
            var objective = await _db.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId, ct);
            if (objective != null)
            {
                objective.Progress = 0;
                objective.Status = "on-track";
                await _db.SaveChangesAsync(ct);
            }
            return;
        }

        // Weighted average calculation
        var totalWeight = keyResults.Sum(kr => kr.Weight);
        var weightedProgress = keyResults.Sum(kr => kr.Progress * kr.Weight);

        var obj = await _db.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId, ct);
        if (obj != null)
        {
            obj.Progress = totalWeight > 0 ? (int)(weightedProgress / totalWeight) : 0;

            // Auto-update status based on progress and due dates
            obj.Status = AutoCalculateStatus(obj.Progress, keyResults);

            await _db.SaveChangesAsync(ct);
        }
    }

    private static string AutoCalculateStatus(int progress, List<KeyResult> keyResults)
    {
        var now = DateTime.UtcNow;
        var overdue = keyResults.Any(kr => kr.DueDate.HasValue && kr.DueDate.Value < now && kr.Progress < 80);

        if (overdue)
        {
            return "off-track";
        }

        if (progress >= 80)
        {
            return "on-track";
        }

        if (progress >= 50)
        {
            return "at-risk";
        }

        return "off-track";
    }
}
