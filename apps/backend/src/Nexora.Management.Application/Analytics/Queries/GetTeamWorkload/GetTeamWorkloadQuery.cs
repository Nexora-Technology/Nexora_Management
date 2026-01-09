using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Analytics.Queries.GetTeamWorkload;

public record GetTeamWorkloadQuery(Guid WorkspaceId) : IRequest<Result<List<TeamWorkloadDto>>>;

public class GetTeamWorkloadQueryHandler : IRequestHandler<GetTeamWorkloadQuery, Result<List<TeamWorkloadDto>>>
{
    private readonly IAppDbContext _db;

    public GetTeamWorkloadQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<TeamWorkloadDto>>> Handle(GetTeamWorkloadQuery request, CancellationToken ct)
    {
        // Single query with LEFT JOINs and aggregation - eliminates N+1 problem
        var teamStats = await _db.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == request.WorkspaceId)
            .Select(wm => new
            {
                wm.UserId,
                UserName = wm.User.Name,
                UserAvatar = wm.User.AvatarUrl,
                RoleName = wm.Role.Name,
                // Task counts
                AssignedTasks = _db.Tasks.Count(t =>
                    t.AssigneeId == wm.UserId &&
                    t.TaskList != null &&
                    t.TaskList.Space != null &&
                    t.TaskList.Space.WorkspaceId == request.WorkspaceId),
                CompletedTasks = _db.Tasks.Count(t =>
                    t.AssigneeId == wm.UserId &&
                    t.Status != null &&
                    t.Status.Name == "complete" &&
                    t.TaskList != null &&
                    t.TaskList.Space != null &&
                    t.TaskList.Space.WorkspaceId == request.WorkspaceId),
                InProgressTasks = _db.Tasks.Count(t =>
                    t.AssigneeId == wm.UserId &&
                    t.Status != null &&
                    t.Status.Name == "inProgress" &&
                    t.TaskList != null &&
                    t.TaskList.Space != null &&
                    t.TaskList.Space.WorkspaceId == request.WorkspaceId),
                // Time entries sum
                TotalHours = _db.TimeEntries
                    .Where(te =>
                        te.UserId == wm.UserId &&
                        te.WorkspaceId == request.WorkspaceId &&
                        te.Status == "approved")
                    .Sum(te => (double?)te.DurationMinutes) / 60.0 ?? 0.0
            })
            .ToListAsync(ct);

        var result = teamStats
            .Select(ts => new TeamWorkloadDto(
                UserId: ts.UserId,
                UserName: ts.UserName ?? "Unknown",
                UserAvatar: ts.UserAvatar,
                AssignedTasks: ts.AssignedTasks,
                CompletedTasks: ts.CompletedTasks,
                InProgressTasks: ts.InProgressTasks,
                CompletionRate: ts.AssignedTasks > 0
                    ? Math.Round((decimal)ts.CompletedTasks / ts.AssignedTasks * 100, 2)
                    : 0,
                TotalHours: (int)Math.Round(ts.TotalHours)
            ))
            .ToList();

        return Result<List<TeamWorkloadDto>>.Success(result);
    }
}
