namespace Nexora.Management.Application.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if request failed
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// List of validation errors or additional error details
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Creates a failed response with error message
    /// </summary>
    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
