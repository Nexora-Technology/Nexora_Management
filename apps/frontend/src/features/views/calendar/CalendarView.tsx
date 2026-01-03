"use client";

import { useState } from "react";
import { format, startOfMonth, endOfMonth, eachDayOfInterval, isSameDay, isSameMonth, addMonths, subMonths } from "date-fns";
import { ChevronLeft, ChevronRight } from "lucide-react";
import type { Task } from "../../tasks/types";

interface CalendarViewProps {
  projectId: string;
}

interface CalendarTask extends Task {
  date: Date;
}

export function CalendarView({ projectId }: CalendarViewProps) {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [tasks] = useState<Task[]>([]); // TODO: Fetch from API

  const monthStart = startOfMonth(currentDate);
  const monthEnd = endOfMonth(currentDate);
  const daysInMonth = eachDayOfInterval({ start: monthStart, end: monthEnd });

  // Get tasks for each day
  const getTasksForDate = (date: Date) => {
    return tasks.filter((task) => {
      if (!task.dueDate) return false;
      return isSameDay(new Date(task.dueDate), date);
    });
  };

  const previousMonth = () => setCurrentDate(subMonths(currentDate, 1));
  const nextMonth = () => setCurrentDate(addMonths(currentDate, 1));

  const weekDays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

  return (
    <div className="calendar-view space-y-4">
      {/* Calendar header */}
      <div className="flex items-center justify-between p-4 border rounded-lg bg-muted/30">
        <button
          onClick={previousMonth}
          className="p-2 hover:bg-muted rounded-lg"
          title="Previous month"
        >
          <ChevronLeft className="h-5 w-5" />
        </button>
        <h2 className="text-lg font-semibold">
          {format(currentDate, "MMMM yyyy")}
        </h2>
        <button
          onClick={nextMonth}
          className="p-2 hover:bg-muted rounded-lg"
          title="Next month"
        >
          <ChevronRight className="h-5 w-5" />
        </button>
      </div>

      {/* Calendar grid */}
      <div className="border rounded-lg overflow-hidden">
        {/* Week day headers */}
        <div className="grid grid-cols-7 bg-muted/50">
          {weekDays.map((day) => (
            <div
              key={day}
              className="px-2 py-2 text-center text-sm font-medium border-b"
            >
              {day}
            </div>
          ))}
        </div>

        {/* Calendar days */}
        <div className="grid grid-cols-7 auto-rows-fr">
          {daysInMonth.map((date) => {
            const dayTasks = getTasksForDate(date);
            const isToday = isSameDay(date, new Date());
            const isCurrentMonth = isSameMonth(date, currentDate);

            return (
              <div
                key={date.toISOString()}
                className={clsx(
                  "min-h-[100px] p-2 border-b border-r last:border-r-0",
                  !isCurrentMonth && "bg-muted/20",
                  isToday && "bg-blue-50/50"
                )}
              >
                <div className={clsx(
                  "text-sm font-medium mb-1",
                  isToday && "text-blue-600"
                )}>
                  {format(date, "d")}
                </div>

                <div className="space-y-1">
                  {dayTasks.slice(0, 3).map((task) => (
                    <div
                      key={task.id}
                      className="text-xs p-1.5 rounded bg-blue-100 text-blue-800 border-l-2 border-blue-500 truncate hover:bg-blue-200 cursor-pointer"
                      title={task.title}
                    >
                      {task.title}
                    </div>
                  ))}
                  {dayTasks.length > 3 && (
                    <div className="text-xs text-muted-foreground text-center">
                      +{dayTasks.length - 3} more
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Task detail modal would go here */}
    </div>
  );
}

function clsx(...classes: (string | boolean | undefined | null)[]) {
  return classes.filter(Boolean).join(" ");
}
