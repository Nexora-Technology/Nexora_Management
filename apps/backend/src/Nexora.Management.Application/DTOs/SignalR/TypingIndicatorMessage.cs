namespace Nexora.Management.Application.DTOs.SignalR;

/// <summary>
/// Typing indicator for collaborative editing
/// </summary>
public class TypingIndicatorMessage
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid TaskId { get; set; }
    public bool IsTyping { get; set; }
    public DateTime Timestamp { get; set; }
}
