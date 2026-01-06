using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.TaskLists.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.TaskLists.Queries.GetTaskLists;

public record GetTaskListsQuery(Guid? SpaceId, Guid? FolderId) : IRequest<Result<List<TaskListDto>>>;

public class GetTaskListsQueryHandler : IRequestHandler<GetTaskListsQuery, Result<List<TaskListDto>>>
{
    private readonly IAppDbContext _db;

    public GetTaskListsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<TaskListDto>>> Handle(GetTaskListsQuery request, CancellationToken ct)
    {
        var query = _db.TaskLists
            .AsNoTracking()
            .AsQueryable();

        if (request.SpaceId.HasValue)
        {
            query = query.Where(tl => tl.SpaceId == request.SpaceId.Value);
        }

        if (request.FolderId.HasValue)
        {
            query = query.Where(tl => tl.FolderId == request.FolderId.Value);
        }

        var taskLists = await query
            .OrderBy(tl => tl.PositionOrder)
            .Select(tl => new TaskListDto(
                tl.Id,
                tl.SpaceId,
                tl.FolderId,
                tl.Name,
                tl.Description,
                tl.Color,
                tl.Icon,
                tl.ListType,
                tl.Status,
                tl.OwnerId,
                tl.PositionOrder,
                tl.CreatedAt,
                tl.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<TaskListDto>>.Success(taskLists);
    }
}
