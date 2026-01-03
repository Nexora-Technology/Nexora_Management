using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Comments.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Comments.Queries.GetComments;

public record GetCommentsQuery(Guid TaskId) : IRequest<Result<List<CommentDto>>>;

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Result<List<CommentDto>>>
{
    private readonly IAppDbContext _db;

    public GetCommentsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<CommentDto>>> Handle(GetCommentsQuery request, CancellationToken ct)
    {
        // Validate task exists
        var taskExists = await _db.Tasks.AnyAsync(t => t.Id == request.TaskId, ct);
        if (!taskExists)
        {
            return Result<List<CommentDto>>.Failure("Task not found");
        }

        var comments = await _db.Comments
            .Where(c => c.TaskId == request.TaskId && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(
                c.Id,
                c.TaskId,
                c.UserId,
                c.User.Name,
                c.User.Email,
                c.Content,
                c.ParentCommentId,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .ToListAsync(ct);

        return Result<List<CommentDto>>.Success(comments);
    }
}
