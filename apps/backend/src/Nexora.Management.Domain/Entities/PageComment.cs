using System.Text.Json;
using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class PageComment : BaseEntity
{
    public Guid PageId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public JsonDocument? Selection { get; set; }
    public Guid? ParentCommentId { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public Page Page { get; set; } = null!;
    public User User { get; set; } = null!;
    public PageComment? ParentComment { get; set; }
    public ICollection<PageComment> Replies { get; set; } = new List<PageComment>();
}
