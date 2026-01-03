using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class WorkspaceMember : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
