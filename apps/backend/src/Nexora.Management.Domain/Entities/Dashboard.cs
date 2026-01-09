using Nexora.Management.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nexora.Management.Domain.Entities;

public class Dashboard : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// JSONB array of DashboardWidget objects: [{id,x,y,w,h,type,title,config},...]
    /// Must be valid JSON array or null
    /// </summary>
    [MaxLength(10000)]
    public string? Layout { get; set; }

    public Guid CreatedBy { get; set; }
    public bool IsTemplate { get; set; } = false;

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public User Creator { get; set; } = null!;
}

/// <summary>
/// Widget layout structure for Gridstack.js dashboard widgets
/// </summary>
public class DashboardWidget
{
    [JsonPropertyName("id")]
    [Required]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("x")]
    [Range(0, 100)]
    public int X { get; set; }

    [JsonPropertyName("y")]
    [Range(0, 100)]
    public int Y { get; set; }

    [JsonPropertyName("w")]
    [Range(1, 12)]
    public int Width { get; set; }

    [JsonPropertyName("h")]
    [Range(1, 20)]
    public int Height { get; set; }

    [JsonPropertyName("type")]
    [Required]
    [RegularExpression("task-status|completion-chart|workload-bar|time-tracking|custom", ErrorMessage = "Invalid widget type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    [MaxLength(200)]
    public string? Title { get; set; }

    [JsonPropertyName("config")]
    public Dictionary<string, object>? Config { get; set; }
}
