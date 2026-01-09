using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Queries.GetActiveTimer;

public record GetActiveTimerQuery() : IRequest<Result<TimeEntryDto?>>;

public class GetActiveTimerQueryHandler : IRequestHandler<GetActiveTimerQuery, Result<TimeEntryDto?>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public GetActiveTimerQueryHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<TimeEntryDto?>> Handle(GetActiveTimerQuery request, CancellationToken ct)
    {
        var entry = await _db.TimeEntries
            .Where(te => te.UserId == _userContext.UserId && te.EndTime == null)
            .Select(te => new TimeEntryDto(
                te.Id,
                te.UserId,
                te.TaskId,
                te.StartTime,
                te.EndTime,
                te.DurationMinutes,
                te.Description,
                te.IsBillable,
                te.Status,
                te.WorkspaceId,
                te.CreatedAt,
                te.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

        return Result<TimeEntryDto?>.Success(entry);
    }
}
