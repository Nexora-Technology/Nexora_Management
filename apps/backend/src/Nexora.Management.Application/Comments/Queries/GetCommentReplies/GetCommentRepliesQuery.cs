using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Comments.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Comments.Queries.GetCommentReplies;

public record GetCommentRepliesQuery(Guid CommentId) : IRequest<Result<List<CommentDto>>>;

public class GetCommentRepliesQueryHandler : IRequestHandler<GetCommentRepliesQuery, Result<List<CommentDto>>>
{
    private readonly IAppDbContext _db;

    public GetCommentRepliesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task<Result<List<CommentDto>>> Handle(GetCommentRepliesQuery request, CancellationToken ct)
    {
        // Validate parent comment exists
        var parentComment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, ct);
        if (parentComment == null)
        {
            return Result<List<CommentDto>>.Failure("Comment not found");
        }

        var replies = await _db.Comments
            .Where(c => c.ParentCommentId == request.CommentId)
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

        return Result<List<CommentDto>>.Success(replies);
    }
}
