using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class Workspace : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public Dictionary<string, object> SettingsJsonb { get; set; } = new Dictionary<string, object>();

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}
