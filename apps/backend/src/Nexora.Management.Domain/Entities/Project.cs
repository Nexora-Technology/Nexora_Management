using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

[Obsolete("Use TaskList instead. Project is kept for backward compatibility during migration to ClickUp hierarchy (Workspace > Space > TaskList > Task).")]
public class Project : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string Status { get; set; } = "active";
    public Guid OwnerId { get; set; }
    public Dictionary<string, object> SettingsJsonb { get; set; } = new Dictionary<string, object>();

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public ICollection<TaskStatus> TaskStatuses { get; set; } = new List<TaskStatus>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
