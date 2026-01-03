using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid WorkspaceId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public Workspace Workspace { get; set; } = null!;
}
