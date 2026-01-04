using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class PageCollaborator
{
    public Guid PageId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "viewer";
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Page Page { get; set; } = null!;
    public User User { get; set; } = null!;
}
