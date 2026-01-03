namespace Nexora.Management.Application.Comments.DTOs;

public record CommentDto(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string UserName,
    string? UserEmail,
    string Content,
    Guid? ParentCommentId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
