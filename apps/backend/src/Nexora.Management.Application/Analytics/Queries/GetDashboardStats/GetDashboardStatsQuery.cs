using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Analytics.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(Guid WorkspaceId) : IRequest<Result<DashboardStatsDto>>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IAppDbContext _db;

    public GetDashboardStatsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        // Get task statistics
        var totalTasks = await _db.Tasks
            .CountAsync(t => t.TaskList != null && t.TaskList.Space != null && t.TaskList.Space.WorkspaceId == request.WorkspaceId, ct);

        var completedTasks = await _db.Tasks
            .CountAsync(t => t.TaskList != null && t.TaskList.Space != null && t.TaskList.Space.WorkspaceId == request.WorkspaceId
                           && t.Status != null && t.Status.Name == "complete", ct);

        var inProgressTasks = await _db.Tasks
            .CountAsync(t => t.TaskList != null && t.TaskList.Space != null && t.TaskList.Space.WorkspaceId == request.WorkspaceId
                           && t.Status != null && t.Status.Name == "inProgress", ct);

        var overdueTasks = await _db.Tasks
            .CountAsync(t => t.TaskList != null && t.TaskList.Space != null && t.TaskList.Space.WorkspaceId == request.WorkspaceId
                           && t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && (t.Status == null || t.Status.Name != "complete"), ct);

        // Get project statistics (TaskLists)
        var totalProjects = await _db.TaskLists
            .CountAsync(tl => tl.Space != null && tl.Space.WorkspaceId == request.WorkspaceId, ct);

        var activeProjects = await _db.TaskLists
            .CountAsync(tl => tl.Space != null && tl.Space.WorkspaceId == request.WorkspaceId
                           && tl.Status == "active", ct);

        // Calculate completion percentage
        var completionPercentage = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;

        // Get team members
        var totalMembers = await _db.WorkspaceMembers
            .CountAsync(wm => wm.WorkspaceId == request.WorkspaceId, ct);

        var activeMembers = await _db.WorkspaceMembers
            .CountAsync(wm => wm.WorkspaceId == request.WorkspaceId
                           && wm.Role.Name != "Guest", ct);

        var stats = new DashboardStatsDto(
            TotalTasks: totalTasks,
            CompletedTasks: completedTasks,
            InProgressTasks: inProgressTasks,
            OverdueTasks: overdueTasks,
            TotalProjects: totalProjects,
            ActiveProjects: activeProjects,
            CompletionPercentage: Math.Round(completionPercentage, 2),
            TotalTeamMembers: totalMembers,
            ActiveMembers: activeMembers
        );

        return Result<DashboardStatsDto>.Success(stats);
    }
}
