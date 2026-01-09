namespace Nexora.Management.Application.Workspaces.DTOs;

public record CreateWorkspaceRequest(
    string Name,
    Dictionary<string, object>? SettingsJsonb
);
