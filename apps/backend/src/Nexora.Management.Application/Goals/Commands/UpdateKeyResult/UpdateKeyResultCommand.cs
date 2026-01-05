using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.UpdateKeyResult;

public record UpdateKeyResultCommand(
    Guid KeyResultId,
    string? Title,
    decimal? CurrentValue,
    decimal? TargetValue,
    DateTime? DueDate,
    int? Weight
) : IRequest<Result<KeyResultDto>>;

public class UpdateKeyResultCommandHandler : IRequestHandler<UpdateKeyResultCommand, Result<KeyResultDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public UpdateKeyResultCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<KeyResultDto>> Handle(UpdateKeyResultCommand request, CancellationToken ct)
    {
        var keyResult = await _db.KeyResults
            .Include(kr => kr.Objective)
            .FirstOrDefaultAsync(kr => kr.Id == request.KeyResultId, ct);

        if (keyResult == null)
        {
            return Result<KeyResultDto>.Failure("Key result not found");
        }

        var objectiveId = keyResult.ObjectiveId;

        // Update fields if provided
        if (request.Title != null)
            keyResult.Title = request.Title;

        if (request.CurrentValue.HasValue)
            keyResult.CurrentValue = request.CurrentValue.Value;

        if (request.TargetValue.HasValue)
            keyResult.TargetValue = request.TargetValue.Value;

        if (request.DueDate.HasValue)
            keyResult.DueDate = request.DueDate.Value;

        if (request.Weight.HasValue)
            keyResult.Weight = request.Weight.Value;

        // Recalculate progress
        keyResult.Progress = CalculateProgress(keyResult.CurrentValue, keyResult.TargetValue);

        await _db.SaveChangesAsync(ct);

        // Update objective progress (weighted average)
        await UpdateObjectiveProgressAsync(objectiveId, ct);

        var keyResultDto = new KeyResultDto(
            keyResult.Id,
            keyResult.ObjectiveId,
            keyResult.Title,
            keyResult.MetricType,
            keyResult.CurrentValue,
            keyResult.TargetValue,
            keyResult.Unit,
            keyResult.DueDate,
            keyResult.Progress,
            keyResult.Weight,
            keyResult.CreatedAt,
            keyResult.UpdatedAt
        );

        return Result<KeyResultDto>.Success(keyResultDto);
    }

    private static int CalculateProgress(decimal current, decimal target)
    {
        if (target == 0) return 0;
        var progress = (current / target) * 100;
        return Math.Max(0, Math.Min(100, (int)progress));
    }

    private async System.Threading.Tasks.Task UpdateObjectiveProgressAsync(Guid objectiveId, CancellationToken ct)
    {
        var keyResults = await _db.KeyResults
            .Where(kr => kr.ObjectiveId == objectiveId)
            .ToListAsync(ct);

        if (!keyResults.Any())
        {
            return;
        }

        // Weighted average calculation
        var totalWeight = keyResults.Sum(kr => kr.Weight);
        var weightedProgress = keyResults.Sum(kr => kr.Progress * kr.Weight);

        var objective = await _db.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId, ct);
        if (objective != null)
        {
            objective.Progress = totalWeight > 0 ? (int)(weightedProgress / totalWeight) : 0;

            // Auto-update status based on progress and due dates
            objective.Status = AutoCalculateStatus(objective.Progress, keyResults);

            await _db.SaveChangesAsync(ct);
        }
    }

    private static string AutoCalculateStatus(int progress, List<KeyResult> keyResults)
    {
        // If all key results have due dates and any are overdue with low progress
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
