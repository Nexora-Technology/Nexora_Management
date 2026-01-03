using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public Guid? WorkspaceId { get; set; }
    public Guid? UserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object>? ChangesJsonb { get; set; }

    // Navigation properties
    public Workspace? Workspace { get; set; }
    public User? User { get; set; }
}
