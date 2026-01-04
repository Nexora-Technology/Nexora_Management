using System.Text.Json;
using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class PageVersion : BaseEntity
{
    public Guid PageId { get; set; }
    public int VersionNumber { get; set; }
    public JsonDocument Content { get; set; } = JsonDocument.Parse("{}");
    public string? CommitMessage { get; set; }
    public Guid CreatedBy { get; set; }

    // Navigation properties
    public Page Page { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
