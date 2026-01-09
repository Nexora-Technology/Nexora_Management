namespace Nexora.Management.Application.Analytics.DTOs;

// Dashboard DTOs
public record DashboardDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string? Layout,
    Guid CreatedBy,
    bool IsTemplate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateDashboardRequest(
    Guid WorkspaceId,
    string Name,
    string? Layout,
    bool IsTemplate = false
);

public record UpdateDashboardRequest(
    string? Name,
    string? Layout
);

// Widget DTOs
public record DashboardWidgetDto(
    string Id,
    int X,
    int Y,
    int Width,
    int Height,
    string Type,
    string? Title,
    Dictionary<string, object>? Config
);

// Chart Data DTOs
public record ChartDataDto(
    string Label,
    decimal Value,
    string? Color
);

public record TimeSeriesDataDto(
    DateTime Date,
    decimal Value,
    string? Label
);

// Dashboard Stats
public record DashboardStatsDto(
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int OverdueTasks,
    int TotalProjects,
    int ActiveProjects,
    decimal CompletionPercentage,
    int TotalTeamMembers,
    int ActiveMembers
);

// Project Progress
public record ProjectProgressDto(
    Guid ProjectId,
    string ProjectName,
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    decimal CompletionPercentage,
    string? Color
);

// Team Workload
public record TeamWorkloadDto(
    Guid UserId,
    string UserName,
    string? UserAvatar,
    int AssignedTasks,
    int CompletedTasks,
    int InProgressTasks,
    decimal CompletionRate,
    int TotalHours
);

// Sprint Burndown
public record SprintBurndownDto(
    DateTime Date,
    int RemainingTasks,
    int CompletedTasks,
    int TotalTasks,
    decimal IdealRemaining,
    decimal ActualRemaining
);

// Report DTOs
public record ReportDto(
    string ReportType,
    string Title,
    DateTime GeneratedAt,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    Dictionary<string, object> Data
);

public record GenerateReportRequest(
    string ReportType, // sprint, project, custom
    DateTime StartDate,
    DateTime EndDate,
    Guid? ProjectId,
    Guid? SprintId,
    Dictionary<string, object>? Filters
);
