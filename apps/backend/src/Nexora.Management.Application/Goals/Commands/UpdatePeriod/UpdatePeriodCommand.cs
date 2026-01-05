using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.UpdatePeriod;

public record UpdatePeriodCommand(
    Guid PeriodId,
    string? Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Status
) : IRequest<Result<GoalPeriodDto>>;

public class UpdatePeriodCommandHandler : IRequestHandler<UpdatePeriodCommand, Result<GoalPeriodDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public UpdatePeriodCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<GoalPeriodDto>> Handle(UpdatePeriodCommand request, CancellationToken ct)
    {
        var period = await _db.GoalPeriods.FirstOrDefaultAsync(p => p.Id == request.PeriodId, ct);
        if (period == null)
        {
            return Result<GoalPeriodDto>.Failure("Goal period not found");
        }

        // Update fields if provided
        if (request.Name != null)
            period.Name = request.Name;

        if (request.StartDate.HasValue)
        {
            period.StartDate = request.StartDate.Value;
            // Validate date range
            if (period.StartDate >= period.EndDate)
            {
                return Result<GoalPeriodDto>.Failure("Start date must be before end date");
            }
        }

        if (request.EndDate.HasValue)
        {
            period.EndDate = request.EndDate.Value;
            // Validate date range
            if (period.StartDate >= period.EndDate)
            {
                return Result<GoalPeriodDto>.Failure("Start date must be before end date");
            }
        }

        if (request.Status != null)
            period.Status = request.Status;

        await _db.SaveChangesAsync(ct);

        var periodDto = new GoalPeriodDto(
            period.Id,
            period.WorkspaceId,
            period.Name,
            period.StartDate,
            period.EndDate,
            period.Status,
            period.CreatedAt,
            period.UpdatedAt
        );

        return Result<GoalPeriodDto>.Success(periodDto);
    }
}
