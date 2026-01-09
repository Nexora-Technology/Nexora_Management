namespace Nexora.Management.Application.Workspaces.DTOs;

public record UpdateWorkspaceRequest(
    string Name,
    Dictionary<string, object>? SettingsJsonb
);
