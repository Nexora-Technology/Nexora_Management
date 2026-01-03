namespace Nexora.Management.Application.Attachments.DTOs;

public record AttachmentDto(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string UserName,
    string FileName,
    long? FileSizeBytes,
    string? MimeType,
    DateTime CreatedAt
);
