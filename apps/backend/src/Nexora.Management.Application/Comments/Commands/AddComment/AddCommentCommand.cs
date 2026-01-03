using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Comments.DTOs;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Comments.Commands.AddComment;

public record AddCommentCommand(
    Guid TaskId,
    string Content,
    Guid? ParentCommentId
) : IRequest<Result<CommentDto>>;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<CommentDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;
    private const int MaxCommentLength = 5000;
    private const int MaxReplyDepth = 5;

    public AddCommentCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<CommentDto>> Handle(AddCommentCommand request, CancellationToken ct)
    {
        // CRITICAL FIX: Validate content length
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<CommentDto>.Failure("Comment content cannot be empty");
        }

        if (request.Content.Length > MaxCommentLength)
        {
            return Result<CommentDto>.Failure($"Comment content exceeds maximum length of {MaxCommentLength} characters");
        }

        // Validate task exists
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId, ct);
        if (task == null)
        {
            return Result<CommentDto>.Failure("Task not found");
        }

        // Validate parent comment if provided
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value, ct);
            if (parentComment == null || parentComment.TaskId != request.TaskId)
            {
                return Result<CommentDto>.Failure("Parent comment not found or belongs to different task");
            }

            // CRITICAL FIX: Prevent unlimited reply depth
            var currentDepth = await GetCommentDepthAsync(parentComment.Id, ct);
            if (currentDepth >= MaxReplyDepth)
            {
                return Result<CommentDto>.Failure($"Maximum reply depth of {MaxReplyDepth} levels exceeded");
            }
        }

        var comment = new Comment
        {
            TaskId = request.TaskId,
            UserId = _userContext.UserId,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(ct);

        // FIX: Include User to prevent N+1 queries
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == comment.UserId, ct);

        var commentDto = new CommentDto(
            comment.Id,
            comment.TaskId,
            comment.UserId,
            user?.Name ?? string.Empty,
            user?.Email,
            comment.Content,
            comment.ParentCommentId,
            comment.CreatedAt,
            comment.UpdatedAt
        );

        return Result<CommentDto>.Success(commentDto);
    }

    private async Task<int> GetCommentDepthAsync(Guid commentId, CancellationToken ct)
    {
        var depth = 0;
        var currentComment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId, ct);

        while (currentComment != null && currentComment.ParentCommentId.HasValue)
        {
            depth++;
            if (depth >= MaxReplyDepth)
            {
                break;
            }
            currentComment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == currentComment.ParentCommentId.Value, ct);
        }

        return depth;
    }
}
