using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Application.Comments.DTOs;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(
    Guid Id,
    string Content
) : IRequest<Result<CommentDto>>;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public UpdateCommentCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken ct)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (comment == null)
        {
            return Result<CommentDto>.Failure("Comment not found");
        }

        // Only the comment author can update it
        if (comment.UserId != _userContext.UserId)
        {
            return Result<CommentDto>.Failure("You can only edit your own comments");
        }

        comment.Content = request.Content;

        await _db.SaveChangesAsync(ct);

        // Get user info for response
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
}
