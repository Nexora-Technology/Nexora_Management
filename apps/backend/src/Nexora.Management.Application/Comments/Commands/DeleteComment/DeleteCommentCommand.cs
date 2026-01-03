using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Management.Application.Common;
using Nexora.Management.Infrastructure.Interfaces;

namespace Nexora.Management.Application.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid Id) : IRequest<Result>;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IUserContext _userContext;

    public DeleteCommentCommandHandler(IAppDbContext db, IUserContext userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async System.Threading.Tasks.Task<Result> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (comment == null)
        {
            return Result.Failure("Comment not found");
        }

        // Only the comment author can delete it
        if (comment.UserId != _userContext.UserId)
        {
            return Result.Failure("You can only delete your own comments");
        }

        // Check if comment has replies
        var hasReplies = await _db.Comments.AnyAsync(c => c.ParentCommentId == request.Id, ct);
        if (hasReplies)
        {
            return Result.Failure("Cannot delete comment with replies. Delete replies first.");
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
