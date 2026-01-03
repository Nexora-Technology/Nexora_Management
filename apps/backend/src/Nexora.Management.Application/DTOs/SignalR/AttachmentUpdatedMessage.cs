namespace Nexora.Management.Application.DTOs.SignalR;

/// <summary>
/// Message broadcasted when an attachment is uploaded or deleted
/// </summary>
public class AttachmentUpdatedMessage
{
    public Guid AttachmentId { get; set; }
    public Guid TaskId { get; set; }
    public string Type { get; set; } = string.Empty; // "uploaded", "deleted"
    public Guid UpdatedBy { get; set; }
    public DateTime Timestamp { get; set; }
    public object? Data { get; set; }
}
