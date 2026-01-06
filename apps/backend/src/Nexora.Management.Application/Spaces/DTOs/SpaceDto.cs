namespace Nexora.Management.Application.Spaces.DTOs;

public record SpaceDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    bool IsPrivate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateSpaceRequest(
    Guid WorkspaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    bool IsPrivate = false
);

public record UpdateSpaceRequest(
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    bool IsPrivate
);
