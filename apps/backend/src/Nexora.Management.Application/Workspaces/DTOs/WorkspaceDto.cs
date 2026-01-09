namespace Nexora.Management.Application.Workspaces.DTOs;

public record WorkspaceDto(
    Guid Id,
    string Name,
    Guid OwnerId,
    string? OwnerName,
    Dictionary<string, object> SettingsJsonb,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
