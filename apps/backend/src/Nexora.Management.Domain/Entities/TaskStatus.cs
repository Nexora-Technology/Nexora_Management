using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class TaskStatus : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int OrderIndex { get; set; }
    public string Type { get; set; } = "open";

    // Navigation properties
    public Project Project { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
