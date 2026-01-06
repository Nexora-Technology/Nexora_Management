using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

/// <summary>
/// Space - First organizational level under Workspace in ClickUp-style hierarchy.
/// Spaces organize work by: departments, teams, clients, or high-level initiatives.
/// Each Space has independent settings and can be private or shared.
/// Hierarchy: Workspace → Space → Folder (optional) → List → Task
/// </summary>
public class Space : BaseEntity
{
    /// <summary>
    /// Workspace this Space belongs to.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Space name (e.g., "Engineering", "Marketing", "Client: Acme Corp").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the Space's purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// UI color for Space display (e.g., "#7B68EE" for ClickUp purple).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Icon identifier for Space display.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Whether this Space is private (visible only to owner) or shared.
    /// </summary>
    public bool IsPrivate { get; set; } = false;

    /// <summary>
    /// Flexible settings storage for Space-level configuration.
    /// </summary>
    public Dictionary<string, object> SettingsJsonb { get; set; } = new Dictionary<string, object>();

    // Navigation properties

    /// <summary>
    /// Parent Workspace.
    /// </summary>
    public Workspace Workspace { get; set; } = null!;

    /// <summary>
    /// Folders contained in this Space.
    /// </summary>
    public ICollection<Folder> Folders { get; set; } = new List<Folder>();

    /// <summary>
    /// TaskLists contained directly in this Space (not in Folders).
    /// Display name in UI: "Lists".
    /// </summary>
    public ICollection<TaskList> TaskLists { get; set; } = new List<TaskList>();
}
