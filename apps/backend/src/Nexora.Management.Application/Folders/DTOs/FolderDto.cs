namespace Nexora.Management.Application.Folders.DTOs;

public record FolderDto(
    Guid Id,
    Guid SpaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    int PositionOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateFolderRequest(
    Guid SpaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon
);

public record UpdateFolderRequest(
    string Name,
    string? Description,
    string? Color,
    string? Icon
);

public record UpdateFolderPositionRequest(
    int PositionOrder
);
