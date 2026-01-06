using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class Task : BaseEntity
{
    // TODO: Migrate to TaskListId, keep ProjectId for backward compatibility during migration
    public Guid ProjectId { get; set; } // DEPRECATED: Use TaskListId after migration
    public Guid TaskListId { get; set; } // NEW: References TaskList in ClickUp hierarchy
    public Guid? ParentTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? StatusId { get; set; }
    public string Priority { get; set; } = "medium";
    public Guid? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public int PositionOrder { get; set; }
    public Dictionary<string, object> CustomFieldsJsonb { get; set; } = new Dictionary<string, object>();
    public Guid CreatedBy { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!; // DEPRECATED: Remove after migration
    public TaskList TaskList { get; set; } = null!; // NEW: TaskList navigation
    public Task? ParentTask { get; set; }
    public ICollection<Task> Subtasks { get; set; } = new List<Task>();
    public User? Assignee { get; set; }
    public User Creator { get; set; } = null!;
    public TaskStatus? Status { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
