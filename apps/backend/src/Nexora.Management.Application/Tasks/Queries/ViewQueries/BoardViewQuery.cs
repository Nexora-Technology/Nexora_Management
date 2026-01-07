using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Tasks.Queries.ViewQueries;

public record GetBoardViewQuery(Guid TaskListId) : IRequest<Result<List<BoardColumnDto>>>;

public class GetBoardViewQueryHandler : IRequestHandler<GetBoardViewQuery, Result<List<BoardColumnDto>>>
{
    private readonly IAppDbContext _db;

    public GetBoardViewQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<BoardColumnDto>>> Handle(
        GetBoardViewQuery request,
        CancellationToken ct)
    {
        // Get all statuses for the tasklist
        var statuses = await _db.TaskStatuses
            .Where(s => s.TaskListId == request.TaskListId)
            .OrderBy(s => s.OrderIndex)
            .ToListAsync(ct);

        // Get all tasks for the tasklist
        var tasks = await _db.Tasks
            .Where(t => t.TaskListId == request.TaskListId)
            .OrderBy(t => t.PositionOrder)
            .Select(t => new TaskDto(
                t.Id,
                t.TaskListId,
                t.ParentTaskId,
                t.Title,
                t.Description,
                t.StatusId,
                t.Priority,
                t.AssigneeId,
                t.DueDate,
                t.StartDate,
                t.EstimatedHours,
                t.PositionOrder,
                t.CreatedBy,
                t.CreatedAt,
                t.UpdatedAt
            ))
            .ToListAsync(ct);

        // Group tasks by status
        var columns = statuses.Select(status => new BoardColumnDto(
            status.Id.ToString(),
            status.Name,
            status.Color,
            status.OrderIndex,
            tasks.Where(t => t.StatusId.HasValue && t.StatusId.Value == status.Id).ToList()
        )).ToList();

        return Result<List<BoardColumnDto>>.Success(columns);
    }
}
