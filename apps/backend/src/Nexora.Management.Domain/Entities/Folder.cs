using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

/// <summary>
/// Folder - Optional grouping container for Lists within a Space.
/// Folders organize related Lists together (e.g., "Sprint 1", "Q4 Campaigns").
/// Folders are single-level only (no sub-folders) and are optional.
/// Hierarchy: Workspace → Space → Folder (optional) → List → Task
/// </summary>
public class Folder : BaseEntity
{
    /// <summary>
    /// Space this Folder belongs to.
    /// Folders always belong to a Space, never directly to Workspace.
    /// </summary>
    public Guid SpaceId { get; set; }

    /// <summary>
    /// Folder name (e.g., "Sprint 1", "Backend Tasks", "Q4 Campaigns").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the Folder's purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// UI color for Folder display.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Icon identifier for Folder display.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Position order for drag-and-drop reordering within the Space.
    /// </summary>
    public int PositionOrder { get; set; } = 0;

    /// <summary>
    /// Flexible settings storage for Folder-level configuration.
    /// </summary>
    public Dictionary<string, object> SettingsJsonb { get; set; } = new Dictionary<string, object>();

    // Navigation properties

    /// <summary>
    /// Parent Space.
    /// </summary>
    public Space Space { get; set; } = null!;

    /// <summary>
    /// TaskLists contained in this Folder.
    /// Display name in UI: "Lists".
    /// </summary>
    public ICollection<TaskList> TaskLists { get; set; } = new List<TaskList>();
}
