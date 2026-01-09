import apiClient from "@/lib/api-client";

export interface TimeEntry {
  id: string;
  userId: string;
  taskId?: string;
  startTime: string;
  endTime?: string;
  durationMinutes: number;
  description?: string;
  isBillable: boolean;
  status: "draft" | "submitted" | "approved" | "rejected";
  workspaceId?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTimeEntryRequest {
  taskId?: string;
  startTime: string;
  endTime?: string;
  durationMinutes: number;
  description?: string;
  isBillable: boolean;
  workspaceId?: string;
}

export interface StartTimeRequest {
  taskId?: string;
  description?: string;
  isBillable: boolean;
  workspaceId?: string;
}

export interface StopTimeRequest {
  description?: string;
}

export interface Timesheet {
  userId: string;
  weekStart: string;
  weekEnd: string;
  dailyTotals: DailyTime[];
  totalMinutes: number;
}

export interface DailyTime {
  date: string;
  totalMinutes: number;
  billableMinutes: number;
  entries: TimeEntry[];
}

export interface TimeReport {
  userId: string;
  periodStart: string;
  periodEnd: string;
  totalMinutes: number;
  billableMinutes: number;
  totalAmount: number;
  taskBreakdown: TaskTimeBreakdown[];
}

export interface TaskTimeBreakdown {
  taskId?: string;
  taskTitle?: string;
  totalMinutes: number;
  entryCount: number;
}

export const timeService = {
  // Timer operations
  async startTimer(request: StartTimeRequest): Promise<TimeEntry> {
    const response = await apiClient.post<TimeEntry>("/api/time/timer/start", request);
    return response.data;
  },

  async stopTimer(request: StopTimeRequest): Promise<TimeEntry> {
    const response = await apiClient.post<TimeEntry>("/api/time/timer/stop", request);
    return response.data;
  },

  async getActiveTimer(): Promise<TimeEntry | null> {
    const response = await apiClient.get<TimeEntry | null>("/api/time/timer/active");
    return response.data;
  },

  // Time entry operations
  async createTimeEntry(request: CreateTimeEntryRequest): Promise<TimeEntry> {
    const response = await apiClient.post<TimeEntry>("/api/time/entries", request);
    return response.data;
  },

  async getTimeEntries(params?: {
    taskId?: string;
    startDate?: string;
    endDate?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<{ data: TimeEntry[]; totalCount: number }> {
    const response = await apiClient.get<{ data: TimeEntry[]; totalCount: number }>(
      "/api/time/entries",
      { params }
    );
    return response.data;
  },

  // Timesheet operations
  async getTimesheet(userId: string, weekStart: string): Promise<Timesheet> {
    const response = await apiClient.get<Timesheet>(`/api/time/timesheet/${userId}`, {
      params: { weekStart },
    });
    return response.data;
  },

  async submitTimesheet(userId: string, weekStart: string, weekEnd: string): Promise<void> {
    await apiClient.post("/api/time/timesheet/submit", {
      userId,
      weekStart,
      weekEnd,
    });
  },

  async approveTimesheet(
    userId: string,
    weekStart: string,
    weekEnd: string,
    status: "approved" | "rejected"
  ): Promise<void> {
    await apiClient.post("/api/time/timesheet/approve", {
      userId,
      weekStart,
      weekEnd,
      status,
    });
  },

  // Reports
  async getTimeReport(
    userId: string,
    periodStart: string,
    periodEnd: string
  ): Promise<TimeReport> {
    const response = await apiClient.get<TimeReport>("/api/time/reports", {
      params: { userId, periodStart, periodEnd },
    });
    return response.data;
  },
};
