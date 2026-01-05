namespace Nexora.Management.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nexora.Management.Domain.Common;

/// <summary>
/// Represents a time period for goal tracking (e.g., Q1 2026, FY 2026)
/// </summary>
public class GoalPeriod : BaseEntity
{
    [Required]
    public Guid WorkspaceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "active"; // active, archived

    // Navigation properties
    [ForeignKey(nameof(WorkspaceId))]
    public Workspace? Workspace { get; set; }

    public ICollection<Objective> Objectives { get; set; } = new List<Objective>();
}

/// <summary>
/// Represents an objective (goal) with hierarchical structure
/// </summary>
public class Objective : BaseEntity
{
    [Required]
    public Guid WorkspaceId { get; set; }

    public Guid? PeriodId { get; set; }

    public Guid? ParentObjectiveId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public Guid? OwnerId { get; set; }

    [Required]
    [Range(1, 10)]
    public int Weight { get; set; } = 1;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "on-track"; // on-track, at-risk, off-track, completed

    /// <summary>
    /// Progress percentage (0-100), calculated from weighted average of key results
    /// </summary>
    [Range(0, 100)]
    public int Progress { get; set; } = 0;

    public int PositionOrder { get; set; }

    // Navigation properties
    [ForeignKey(nameof(WorkspaceId))]
    public Workspace? Workspace { get; set; }

    [ForeignKey(nameof(PeriodId))]
    public GoalPeriod? Period { get; set; }

    [ForeignKey(nameof(ParentObjectiveId))]
    public Objective? ParentObjective { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }

    public ICollection<Objective> SubObjectives { get; set; } = new List<Objective>();
    public ICollection<KeyResult> KeyResults { get; set; } = new List<KeyResult>();
}

/// <summary>
/// Represents a measurable key result for an objective
/// </summary>
public class KeyResult : BaseEntity
{
    [Required]
    public Guid ObjectiveId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string MetricType { get; set; } = string.Empty; // number, percentage, currency

    [Required]
    public decimal CurrentValue { get; set; }

    [Required]
    public decimal TargetValue { get; set; }

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty; // %, $, count, etc.

    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Progress percentage (0-100), calculated as (CurrentValue / TargetValue) * 100
    /// </summary>
    [Range(0, 100)]
    public int Progress { get; set; } = 0;

    [Required]
    [Range(1, 10)]
    public int Weight { get; set; } = 1; // For weighted average calculation

    // Navigation properties
    [ForeignKey(nameof(ObjectiveId))]
    public Objective? Objective { get; set; }
}
