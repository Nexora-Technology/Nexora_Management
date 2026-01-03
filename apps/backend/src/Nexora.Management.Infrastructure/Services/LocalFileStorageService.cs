using Microsoft.Extensions.Configuration;

namespace Nexora.Management.Infrastructure.Services;

/// <summary>
/// Local filesystem implementation of file storage
/// In production, consider using Azure Blob Storage, AWS S3, or similar
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseStoragePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _baseStoragePath = configuration["Storage:BasePath"] ?? Path.Combine(Path.GetTempPath(), "nexora-uploads");

        // Ensure directory exists
        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        // CRITICAL FIX: Prevent path traversal by extracting only the filename
        var safeFileName = Path.GetFileName(fileName);

        // Validate filename is not empty after sanitization
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new ArgumentException("Invalid filename", nameof(fileName));
        }

        // Create unique filename with timestamp to avoid conflicts
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniqueFileName = $"{timestamp}_{safeFileName}";
        var filePath = Path.Combine(_baseStoragePath, uniqueFileName);

        // Save file
        using var fileStreamTarget = File.Create(filePath);
        await fileStream.CopyToAsync(fileStreamTarget, ct);

        return filePath;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken ct = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }

    public async Task<(Stream FileStream, string ContentType)> GetFileAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        var fileStream = File.OpenRead(filePath);
        var contentType = GetMimeType(filePath);

        return await Task.FromResult((fileStream, contentType));
    }

    private static string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}
