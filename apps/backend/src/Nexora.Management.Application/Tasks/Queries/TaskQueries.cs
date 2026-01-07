using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Tasks.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;
using DomainTask = Nexora.Management.Domain.Entities.Task;

namespace Nexora.Management.Application.Tasks.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskDto>>;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IAppDbContext _db;

    public GetTaskByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken ct)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.Id, ct);

        if (task == null)
        {
            return Result<TaskDto>.Failure("Task not found");
        }

        var taskDto = new TaskDto(
            task.Id,
            task.TaskListId,
            task.ParentTaskId,
            task.Title,
            task.Description,
            task.StatusId,
            task.Priority,
            task.AssigneeId,
            task.DueDate,
            task.StartDate,
            task.EstimatedHours,
            task.PositionOrder,
            task.CreatedBy,
            task.CreatedAt,
            task.UpdatedAt
        );

        return Result<TaskDto>.Success(taskDto);
    }
}

public record GetTasksQuery(
    Guid? TaskListId,
    Guid? StatusId,
    Guid? AssigneeId,
    string? Search,
    string? SortBy,
    bool SortDesc,
    int Page,
    int PageSize
) : IRequest<Result<PagedResponse<TaskDto>>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<PagedResponse<TaskDto>>>
{
    private readonly IAppDbContext _db;

    public GetTasksQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<PagedResponse<TaskDto>>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var query = _db.Tasks.AsQueryable();

        // Filters
        if (request.TaskListId.HasValue)
        {
            query = query.Where(t => t.TaskListId == request.TaskListId);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(t => t.StatusId == request.StatusId);
        }

        if (request.AssigneeId.HasValue)
        {
            query = query.Where(t => t.AssigneeId == request.AssigneeId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(t =>
                t.Title.Contains(request.Search) ||
                (t.Description != null && t.Description.Contains(request.Search))
            );
        }

        // Get total count
        var totalCount = await query.CountAsync(ct);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "duedate" => request.SortDesc
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
            "priority" => request.SortDesc
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),
            "createdat" => request.SortDesc
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
            _ => query.OrderBy(t => t.PositionOrder)
        };

        // Pagination
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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

        var pagedResponse = new PagedResponse<TaskDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result<PagedResponse<TaskDto>>.Success(pagedResponse);
    }
}
