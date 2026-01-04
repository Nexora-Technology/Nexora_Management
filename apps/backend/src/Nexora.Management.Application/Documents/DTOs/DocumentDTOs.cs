using System.Text.Json;

namespace Nexora.Management.Application.Documents.DTOs;

public record PageDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? ParentPageId,
    string Title,
    string Slug,
    string? Icon,
    string? CoverImage,
    string ContentType,
    string Status,
    bool IsFavorite,
    int PositionOrder,
    Guid CreatedBy,
    Guid UpdatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PageDetailDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? ParentPageId,
    string Title,
    string Slug,
    string? Icon,
    string? CoverImage,
    JsonDocument Content,
    string ContentType,
    string Status,
    bool IsFavorite,
    int PositionOrder,
    Guid CreatedBy,
    string? CreatedByName,
    Guid UpdatedBy,
    string? UpdatedByName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PageTreeNodeDto(
    Guid Id,
    Guid? ParentPageId,
    string Title,
    string Slug,
    string? Icon,
    string ContentType,
    string Status,
    bool IsFavorite,
    int PositionOrder,
    List<PageTreeNodeDto> Children
)
{
    public PageTreeNodeDto() : this(Guid.Empty, null, string.Empty, string.Empty, null, string.Empty, string.Empty, false, 0, new List<PageTreeNodeDto>())
    {
    }
}

public record PageVersionDto(
    Guid Id,
    Guid PageId,
    int VersionNumber,
    JsonDocument Content,
    string? CommitMessage,
    Guid CreatedBy,
    string? CreatedByName,
    DateTime CreatedAt
);

public record PageCommentDto(
    Guid Id,
    Guid PageId,
    Guid UserId,
    string? UserName,
    string Content,
    JsonDocument? Selection,
    Guid? ParentCommentId,
    DateTime CreatedAt,
    DateTime? ResolvedAt
);

public record CreatePageRequest(
    Guid WorkspaceId,
    string Title,
    Guid? ParentPageId,
    string? Icon,
    string? ContentType
);

public record UpdatePageRequest(
    string Title,
    JsonDocument Content,
    string? Icon,
    string? CoverImage
);

public record MovePageRequest(
    Guid? NewParentPageId,
    int NewPositionOrder
);

public record RestoreVersionRequest(
    int VersionNumber
);

public record SearchPagesRequest(
    Guid WorkspaceId,
    string? SearchTerm,
    string? Status,
    bool? FavoriteOnly,
    int Page = 1,
    int PageSize = 20
);
