using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Analytics.DTOs;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Analytics.Queries.GetProjectProgress;

public record GetProjectProgressQuery(Guid WorkspaceId) : IRequest<Result<List<ProjectProgressDto>>>;

public class GetProjectProgressQueryHandler : IRequestHandler<GetProjectProgressQuery, Result<List<ProjectProgressDto>>>
{
    private readonly IAppDbContext _db;

    public GetProjectProgressQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ProjectProgressDto>>> Handle(GetProjectProgressQuery request, CancellationToken ct)
    {
        // Single query with aggregation - eliminates N+1 problem
        var projectStats = await _db.TaskLists
            .Where(tl => tl.Space != null && tl.Space.WorkspaceId == request.WorkspaceId)
            .Select(tl => new
            {
                tl.Id,
                tl.Name,
                tl.Color,
                TotalTasks = _db.Tasks.Count(t => t.TaskListId == tl.Id),
                CompletedTasks = _db.Tasks.Count(t => t.TaskListId == tl.Id && t.Status != null && t.Status.Name == "complete"),
                InProgressTasks = _db.Tasks.Count(t => t.TaskListId == tl.Id && t.Status != null && t.Status.Name == "inProgress")
            })
            .ToListAsync(ct);

        var result = projectStats
            .Select(ps => new ProjectProgressDto(
                ProjectId: ps.Id,
                ProjectName: ps.Name,
                TotalTasks: ps.TotalTasks,
                CompletedTasks: ps.CompletedTasks,
                InProgressTasks: ps.InProgressTasks,
                CompletionPercentage: ps.TotalTasks > 0
                    ? Math.Round((decimal)ps.CompletedTasks / ps.TotalTasks * 100, 2)
                    : 0,
                Color: ps.Color
            ))
            .ToList();

        return Result<List<ProjectProgressDto>>.Success(result);
    }
}
