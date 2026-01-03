namespace Nexora.Management.Infrastructure.Services;

/// <summary>
/// Service for handling file storage operations
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file and returns the stored file path
    /// </summary>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, CancellationToken ct = default);

    /// <summary>
    /// Deletes a file by its path
    /// </summary>
    Task DeleteFileAsync(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Gets a file stream for download
    /// </summary>
    Task<(Stream FileStream, string ContentType)> GetFileAsync(string filePath, CancellationToken ct = default);
}
