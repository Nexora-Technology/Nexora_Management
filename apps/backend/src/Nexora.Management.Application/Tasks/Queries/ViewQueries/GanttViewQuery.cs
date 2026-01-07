using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Tasks.Queries.ViewQueries;

public record GetGanttViewQuery(Guid TaskListId) : IRequest<Result<List<GanttTaskDto>>>;

public class GetGanttViewQueryHandler : IRequestHandler<GetGanttViewQuery, Result<List<GanttTaskDto>>>
{
    private readonly IAppDbContext _db;

    public GetGanttViewQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<GanttTaskDto>>> Handle(
        GetGanttViewQuery request,
        CancellationToken ct)
    {
        // Get all tasks for the tasklist
        var allTasks = await _db.Tasks
            .Where(t => t.TaskListId == request.TaskListId)
            .OrderBy(t => t.PositionOrder)
            .ToListAsync(ct);

        // Build hierarchical structure
        var rootTasks = allTasks.Where(t => t.ParentTaskId == null).ToList();
        var result = new List<GanttTaskDto>();

        foreach (var task in rootTasks)
        {
            var ganttTask = BuildGanttTask(task, allTasks);
            result.Add(ganttTask);
        }

        return Result<List<GanttTaskDto>>.Success(result);
    }

    private GanttTaskDto BuildGanttTask(Domain.Entities.Task task, List<Domain.Entities.Task> allTasks)
    {
        // Calculate duration
        int duration = 0;
        if (task.StartDate.HasValue && task.DueDate.HasValue)
        {
            duration = (int)(task.DueDate.Value - task.StartDate.Value).TotalDays + 1;
        }

        // Find children
        var children = allTasks
            .Where(t => t.ParentTaskId == task.Id)
            .Select(childTask => BuildGanttTask(childTask, allTasks))
            .ToList();

        return new GanttTaskDto(
            task.Id,
            task.Title,
            task.StartDate,
            task.DueDate,
            duration,
            task.StatusId,
            task.Priority,
            task.ParentTaskId,
            children
        );
    }
}
