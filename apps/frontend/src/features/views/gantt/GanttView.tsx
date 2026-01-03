"use client";

import { useState, useMemo } from "react";
import { format, differenceInDays, addDays, min, max, isSameDay } from "date-fns";
import { clsx } from "clsx";
import type { Task } from "../../tasks/types";

interface GanttViewProps {
  projectId: string;
}

interface GanttTask {
  id: string;
  title: string;
  startDate: Date;
  endDate: Date;
  children: GanttTask[];
}

export function GanttView({ projectId }: GanttViewProps) {
  const [tasks] = useState<Task[]>([]); // TODO: Fetch from API
  const [zoomLevel, setZoomLevel] = useState<"day" | "week" | "month">("day");

  // Convert tasks to Gantt format and calculate date range
  const { timelineStart, timelineEnd, ganttTasks } = useMemo(() => {
    if (tasks.length === 0) {
      const now = new Date();
      return {
        timelineStart: addDays(now, -7),
        timelineEnd: addDays(now, 30),
        ganttTasks: [],
      };
    }

    // Calculate date range from all tasks
    const allDates = tasks.flatMap((task) => {
      const dates = [];
      if (task.startDate) dates.push(new Date(task.startDate));
      if (task.dueDate) dates.push(new Date(task.dueDate));
      return dates;
    });

    if (allDates.length === 0) {
      const now = new Date();
      return {
        timelineStart: addDays(now, -7),
        timelineEnd: addDays(now, 30),
        ganttTasks: [],
      };
    }

    const timelineStart = min(allDates);
    const timelineEnd = max(allDates);

    // Ensure at least 30 days range
    const adjustedEnd = differenceInDays(timelineEnd, timelineStart) < 30
      ? addDays(timelineStart, 30)
      : timelineEnd;

    // Add buffer days
    const start = addDays(timelineStart, -3);
    const end = addDays(adjustedEnd, 7);

    const ganttTasks: GanttTask[] = tasks.map((task) => ({
      ...task,
      startDate: task.startDate ? new Date(task.startDate) : start,
      endDate: task.dueDate ? new Date(task.dueDate) : end,
      children: [], // TODO: Build hierarchy from parentTaskId
    }));

    return { timelineStart: start, timelineEnd: end, ganttTasks };
  }, [tasks]);

  // Generate timeline headers based on zoom level
  const timelineDays = useMemo(() => {
    const days = differenceInDays(timelineEnd, timelineStart);
    return Array.from({ length: days + 1 }, (_, i) => addDays(timelineStart, i));
  }, [timelineStart, timelineEnd]);

  const getDayWidth = () => {
    switch (zoomLevel) {
      case "day":
        return 40; // 40px per day
      case "week":
        return 10; // 10px per day (70px per week)
      case "month":
        return 3; // 3px per day (~90px per month)
    }
  };

  const dayWidth = getDayWidth();
  const totalWidth = timelineDays.length * dayWidth;

  const getTaskPosition = (task: GanttTask) => {
    const startOffset = differenceInDays(task.startDate, timelineStart);
    const duration = differenceInDays(task.endDate, task.startDate) + 1;
    return {
      left: startOffset * dayWidth,
      width: duration * dayWidth,
    };
  };

  return (
    <div className="gantt-view space-y-4">
      {/* Zoom controls */}
      <div className="flex items-center gap-2 p-4 border rounded-lg bg-muted/30">
        <span className="text-sm text-muted-foreground">Zoom:</span>
        <button
          onClick={() => setZoomLevel("day")}
          className={clsx(
            "px-3 py-1 rounded text-sm",
            zoomLevel === "day" ? "bg-primary text-primary-foreground" : "bg-secondary"
          )}
        >
          Day
        </button>
        <button
          onClick={() => setZoomLevel("week")}
          className={clsx(
            "px-3 py-1 rounded text-sm",
            zoomLevel === "week" ? "bg-primary text-primary-foreground" : "bg-secondary"
          )}
        >
          Week
        </button>
        <button
          onClick={() => setZoomLevel("month")}
          className={clsx(
            "px-3 py-1 rounded text-sm",
            zoomLevel === "month" ? "bg-primary text-primary-foreground" : "bg-secondary"
          )}
        >
          Month
        </button>
      </div>

      {/* Gantt chart */}
      <div className="border rounded-lg overflow-x-auto">
        <div className="min-w-[800px]">
          {/* Header with dates */}
          <div className="flex border-b">
            <div className="w-64 flex-shrink-0 p-3 font-medium bg-muted/50 border-r">
              Task
            </div>
            <div className="flex" style={{ width: `${totalWidth}px` }}>
              {timelineDays.map((date, index) => {
                const showLabel = zoomLevel === "day" ||
                  (zoomLevel === "week" && date.getDay() === 0) ||
                  (zoomLevel === "month" && date.getDate() === 1);

                if (!showLabel && index > 0) return null;

                return (
                  <div
                    key={date.toISOString()}
                    className="flex-shrink-0 p-3 text-xs text-center border-r"
                    style={{ width: zoomLevel === "week" && index === 0 ? "140px" : undefined }}
                  >
                    {zoomLevel === "day" && format(date, "MMM d")}
                    {zoomLevel === "week" && format(date, "MMM d")}
                    {zoomLevel === "month" && format(date, "MMM")}
                  </div>
                );
              })}
            </div>
          </div>

          {/* Tasks */}
          {ganttTasks.map((task) => {
            const { left, width } = getTaskPosition(task);
            const isToday = timelineDays.some((day) =>
              isSameDay(day, new Date()) &&
              day >= task.startDate &&
              day <= task.endDate
            );

            return (
              <div key={task.id} className="flex border-b">
                <div className="w-64 flex-shrink-0 p-3 border-r">
                  <div className="font-medium text-sm truncate">{task.title}</div>
                  <div className="text-xs text-muted-foreground">
                    {format(task.startDate, "MMM d")} - {format(task.endDate, "MMM d")}
                  </div>
                </div>
                <div className="relative py-3" style={{ width: `${totalWidth}px` }}>
                  {/* Grid lines */}
                  {timelineDays.map((date) => (
                    <div
                      key={date.toISOString()}
                      className="absolute top-0 bottom-0 border-r border-muted/30"
                      style={{ left: `${differenceInDays(date, timelineStart) * dayWidth}px`, width: `${dayWidth}px` }}
                    />
                  ))}

                  {/* Task bar */}
                  <div
                    className={clsx(
                      "absolute top-1/2 -translate-y-1/2 h-6 rounded-full text-xs flex items-center px-2 text-white truncate",
                      isToday ? "bg-blue-600" : "bg-blue-500"
                    )}
                    style={{ left: `${left}px`, width: `${width}px` }}
                  >
                    {width > 50 && task.title}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
