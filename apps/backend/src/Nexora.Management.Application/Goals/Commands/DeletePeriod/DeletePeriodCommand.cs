using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.DeletePeriod;

public record DeletePeriodCommand(Guid PeriodId) : IRequest<Result>;

public class DeletePeriodCommandHandler : IRequestHandler<DeletePeriodCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public DeletePeriodCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeletePeriodCommand request, CancellationToken ct)
    {
        var period = await _db.GoalPeriods
            .Include(p => p.Objectives)
            .FirstOrDefaultAsync(p => p.Id == request.PeriodId, ct);

        if (period == null)
        {
            return Result.Failure("Goal period not found");
        }

        // Check if period has objectives
        if (period.Objectives.Any())
        {
            return Result.Failure("Cannot delete period with associated objectives. Reassign or delete objectives first.");
        }

        _db.GoalPeriods.Remove(period);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
