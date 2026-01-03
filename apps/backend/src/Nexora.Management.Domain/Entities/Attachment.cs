using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class Attachment : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long? FileSizeBytes { get; set; }
    public string? MimeType { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public User User { get; set; } = null!;
}
