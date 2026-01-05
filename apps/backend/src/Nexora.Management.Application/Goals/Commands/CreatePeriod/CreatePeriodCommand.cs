using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Goals.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Goals.Commands.CreatePeriod;

public record CreatePeriodCommand(
    Guid WorkspaceId,
    string Name,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<GoalPeriodDto>>;

public class CreatePeriodCommandHandler : IRequestHandler<CreatePeriodCommand, Result<GoalPeriodDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public CreatePeriodCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<GoalPeriodDto>> Handle(CreatePeriodCommand request, CancellationToken ct)
    {
        // Validate workspace exists
        var workspace = await _db.Workspaces.FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, ct);
        if (workspace == null)
        {
            return Result<GoalPeriodDto>.Failure("Workspace not found");
        }

        // Validate date range
        if (request.StartDate >= request.EndDate)
        {
            return Result<GoalPeriodDto>.Failure("Start date must be before end date");
        }

        var period = new GoalPeriod
        {
            WorkspaceId = request.WorkspaceId,
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "active"
        };

        _db.GoalPeriods.Add(period);
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
