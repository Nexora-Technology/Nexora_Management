namespace Nexora.Management.Application.Goals.DTOs;

/// <summary>
/// Data transfer object for GoalPeriod entity
/// </summary>
public record GoalPeriodDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Data transfer object for Objective entity
/// </summary>
public record ObjectiveDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string Title,
    string? Description,
    Guid? OwnerId,
    int Weight,
    string Status,
    int Progress,
    int PositionOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Data transfer object for KeyResult entity
/// </summary>
public record KeyResultDto(
    Guid Id,
    Guid ObjectiveId,
    string Title,
    string MetricType,
    decimal CurrentValue,
    decimal TargetValue,
    string Unit,
    DateTime? DueDate,
    int Progress,
    int Weight,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Tree node representation for hierarchical objectives
/// </summary>
public record ObjectiveTreeNodeDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string Title,
    string? Description,
    Guid? OwnerId,
    string? OwnerName,
    string? OwnerEmail,
    int Weight,
    string Status,
    int Progress,
    int PositionOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<KeyResultDto> KeyResults,
    List<ObjectiveTreeNodeDto> SubObjectives
);

/// <summary>
/// Summary statistics for progress dashboard
/// </summary>
public record ProgressDashboardDto(
    int TotalObjectives,
    int AverageProgress,
    int TotalKeyResults,
    int CompletedKeyResults,
    List<StatusBreakdownDto> StatusBreakdown,
    List<ObjectiveSummaryDto> TopObjectives,
    List<ObjectiveSummaryDto> BottomObjectives
);

/// <summary>
/// Breakdown of objectives by status
/// </summary>
public record StatusBreakdownDto(
    string Status,
    int Count,
    double Percentage
);

/// <summary>
/// Summary representation of an objective for dashboard
/// </summary>
public record ObjectiveSummaryDto(
    Guid Id,
    string Title,
    string Status,
    int Progress,
    int KeyResultCount,
    Guid? OwnerId
);
