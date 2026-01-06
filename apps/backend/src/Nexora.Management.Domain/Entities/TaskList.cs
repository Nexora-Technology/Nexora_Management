using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

/// <summary>
/// TaskList - Mandatory container for Tasks in ClickUp-style hierarchy.
/// "TaskList" used instead of "List" to avoid C# reserved keyword conflict.
/// Display name in UI: "List".
///
/// TaskLists track different content types: tasks, projects, teams, campaigns, etc.
/// TaskLists can exist directly under Spaces or within Folders.
/// Every Task MUST belong to a TaskList (no orphaned tasks).
///
/// Hierarchy: Workspace → Space → Folder (optional) → TaskList → Task
/// </summary>
public class TaskList : BaseEntity
{
    /// <summary>
    /// Space this TaskList belongs to (required).
    /// </summary>
    public Guid SpaceId { get; set; }

    /// <summary>
    /// Optional Folder this TaskList belongs to.
    /// NULL if TaskList exists directly under Space.
    /// </summary>
    public Guid? FolderId { get; set; }

    /// <summary>
    /// TaskList name (e.g., "API Tasks", "Sprint 1 Backlog", "Marketing Campaigns").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the TaskList's purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// UI color for TaskList display.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Icon identifier for TaskList display.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Type of content this TaskList tracks.
    /// Examples: "task", "project", "team", "campaign", "milestone".
    /// Enables TaskLists to track different content types per ClickUp's model.
    /// </summary>
    public string ListType { get; set; } = "task";

    /// <summary>
    /// Status of the TaskList (e.g., "active", "archived", "completed").
    /// </summary>
    public string Status { get; set; } = "active";

    /// <summary>
    /// User who owns this TaskList.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Position order for drag-and-drop reordering within Space/Folder.
    /// </summary>
    public int PositionOrder { get; set; } = 0;

    /// <summary>
    /// Flexible settings storage for TaskList-level configuration.
    /// </summary>
    public Dictionary<string, object> SettingsJsonb { get; set; } = new Dictionary<string, object>();

    // Navigation properties

    /// <summary>
    /// Parent Space.
    /// </summary>
    public Space Space { get; set; } = null!;

    /// <summary>
    /// Optional parent Folder.
    /// </summary>
    public Folder? Folder { get; set; }

    /// <summary>
    /// User who owns this TaskList.
    /// </summary>
    public User Owner { get; set; } = null!;

    /// <summary>
    /// Tasks contained in this TaskList.
    /// </summary>
    public ICollection<Task> Tasks { get; set; } = new List<Task>();

    /// <summary>
    /// Task statuses configured for this TaskList.
    /// </summary>
    public ICollection<TaskStatus> TaskStatuses { get; set; } = new List<TaskStatus>();
}
