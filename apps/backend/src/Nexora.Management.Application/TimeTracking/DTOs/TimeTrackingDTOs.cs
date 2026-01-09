namespace Nexora.Management.Application.TimeTracking.DTOs;

public record TimeEntryDto(
    Guid Id,
    Guid UserId,
    Guid? TaskId,
    DateTime StartTime,
    DateTime? EndTime,
    int DurationMinutes,
    string? Description,
    bool IsBillable,
    string Status,
    Guid? WorkspaceId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTimeEntryRequest(
    Guid? TaskId,
    DateTime StartTime,
    DateTime? EndTime,
    int DurationMinutes,
    string? Description,
    bool IsBillable,
    Guid? WorkspaceId
);

public record UpdateTimeEntryRequest(
    DateTime? StartTime,
    DateTime? EndTime,
    int? DurationMinutes,
    string? Description,
    bool? IsBillable
);

public record StartTimeRequest(
    Guid? TaskId,
    string? Description,
    bool IsBillable,
    Guid? WorkspaceId
);

public record StopTimeRequest(
    string? Description
);

public record GetTimeEntriesQueryRequest(
    Guid? TaskId,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Status,
    int Page = 1,
    int PageSize = 50
);

public record TimesheetDto(
    Guid UserId,
    DateTime WeekStart,
    DateTime WeekEnd,
    List<DailyTimeDto> DailyTotals,
    int TotalMinutes
);

public record DailyTimeDto(
    DateTime Date,
    int TotalMinutes,
    int BillableMinutes,
    List<TimeEntryDto> Entries
);

public record TimeReportDto(
    Guid UserId,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    int TotalMinutes,
    int BillableMinutes,
    decimal TotalAmount,
    List<TaskTimeBreakdownDto> TaskBreakdown
);

public record TaskTimeBreakdownDto(
    Guid? TaskId,
    string? TaskTitle,
    int TotalMinutes,
    int EntryCount
);

public record TimesheetSubmitRequest(
    Guid UserId,
    DateTime WeekStart,
    DateTime WeekEnd
);

public record TimesheetApprovalRequest(
    Guid UserId,
    DateTime WeekStart,
    DateTime WeekEnd,
    string Status // "approved" or "rejected"
);
