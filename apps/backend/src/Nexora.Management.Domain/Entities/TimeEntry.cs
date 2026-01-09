using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string? Description { get; set; }
    public bool IsBillable { get; set; }
    public string Status { get; set; } = "draft"; // draft, submitted, approved, rejected
    public Guid? WorkspaceId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Task? Task { get; set; }
    public Workspace? Workspace { get; set; }
}
