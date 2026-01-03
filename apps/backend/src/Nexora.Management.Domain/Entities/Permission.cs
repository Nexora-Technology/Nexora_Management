using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class Permission : BaseEntity
{
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
