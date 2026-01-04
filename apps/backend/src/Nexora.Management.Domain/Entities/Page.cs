using System.Text.Json;
using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class Page : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public Guid? ParentPageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? CoverImage { get; set; }
    public JsonDocument Content { get; set; } = JsonDocument.Parse("{}");
    public string ContentType { get; set; } = "rich-text";
    public string Status { get; set; } = "active";
    public bool IsFavorite { get; set; }
    public int PositionOrder { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public Page? ParentPage { get; set; }
    public ICollection<Page> SubPages { get; set; } = new List<Page>();
    public ICollection<PageVersion> Versions { get; set; } = new List<PageVersion>();
    public ICollection<PageCollaborator> Collaborators { get; set; } = new List<PageCollaborator>();
    public ICollection<PageComment> Comments { get; set; } = new List<PageComment>();
    public User Creator { get; set; } = null!;
    public User Updater { get; set; } = null!;
}
