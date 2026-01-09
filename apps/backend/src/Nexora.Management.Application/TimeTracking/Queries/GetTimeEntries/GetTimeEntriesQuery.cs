using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TimeTracking.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TimeTracking.Queries.GetTimeEntries;

public record GetTimeEntriesQuery(
    Guid? TaskId,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Status,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PagedResult<TimeEntryDto>>>;

public class GetTimeEntriesQueryHandler : IRequestHandler<GetTimeEntriesQuery, Result<PagedResult<TimeEntryDto>>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public GetTimeEntriesQueryHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<PagedResult<TimeEntryDto>>> Handle(GetTimeEntriesQuery request, CancellationToken ct)
    {
        var query = _db.TimeEntries
            .Where(te => te.UserId == _userContext.UserId);

        // Apply filters
        if (request.TaskId.HasValue)
        {
            query = query.Where(te => te.TaskId == request.TaskId.Value);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(te => te.StartTime >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            var endDate = request.EndDate.Value.AddDays(1).AddSeconds(-1);
            query = query.Where(te => te.StartTime <= endDate);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(te => te.Status == request.Status);
        }

        // Get total count
        var totalCount = await query.CountAsync(ct);

        // Apply pagination
        var entries = await query
            .OrderByDescending(te => te.StartTime)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(ct);

        var pagedResult = new PagedResult<TimeEntryDto>(
            entries,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result<PagedResult<TimeEntryDto>>.Success(pagedResult);
    }
}
