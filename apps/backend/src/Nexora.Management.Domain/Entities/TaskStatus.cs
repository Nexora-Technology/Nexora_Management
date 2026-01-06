using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class TaskStatus : BaseEntity
{
    // TODO: Migrate to TaskListId, keep ProjectId for backward compatibility during migration
    public Guid ProjectId { get; set; } // DEPRECATED: Use TaskListId after migration
    public Guid TaskListId { get; set; } // NEW: References TaskList in ClickUp hierarchy
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int OrderIndex { get; set; }
    public string Type { get; set; } = "open";

    // Navigation properties
    public Project Project { get; set; } = null!; // DEPRECATED: Remove after migration
    public TaskList TaskList { get; set; } = null!; // NEW: TaskList navigation
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
