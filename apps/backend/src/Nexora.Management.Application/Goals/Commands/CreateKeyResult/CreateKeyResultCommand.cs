using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.CreateKeyResult;

public record CreateKeyResultCommand(
    Guid ObjectiveId,
    string Title,
    string MetricType,
    decimal CurrentValue,
    decimal TargetValue,
    string Unit,
    DateTime? DueDate,
    int Weight
) : IRequest<Result<KeyResultDto>>;

public class CreateKeyResultCommandHandler : IRequestHandler<CreateKeyResultCommand, Result<KeyResultDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreateKeyResultCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<KeyResultDto>> Handle(CreateKeyResultCommand request, CancellationToken ct)
    {
        var objective = await _db.Objectives.FirstOrDefaultAsync(o => o.Id == request.ObjectiveId, ct);
        if (objective == null)
        {
            return Result<KeyResultDto>.Failure("Objective not found");
        }

        var keyResult = new KeyResult
        {
            ObjectiveId = request.ObjectiveId,
            Title = request.Title,
            MetricType = request.MetricType,
            CurrentValue = request.CurrentValue,
            TargetValue = request.TargetValue,
            Unit = request.Unit,
            DueDate = request.DueDate,
            Weight = request.Weight,
            Progress = CalculateProgress(request.CurrentValue, request.TargetValue)
        };

        _db.KeyResults.Add(keyResult);
        await _db.SaveChangesAsync(ct);

        // Update objective progress (weighted average of key results)
        await UpdateObjectiveProgressAsync(request.ObjectiveId, ct);

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
