"use client";

import { useState, useEffect } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { timeService, Timesheet, DailyTime } from "@/lib/services/time-service";
import { format, startOfWeek, addDays, isSameDay } from "date-fns";
import { ChevronLeft, ChevronRight, Download } from "lucide-react";

export function TimesheetView() {
  const [currentWeek, setCurrentWeek] = useState(new Date());
  const [timesheet, setTimesheet] = useState<Timesheet | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadTimesheet();
  }, [currentWeek]);

  const loadTimesheet = async () => {
    setLoading(true);
    try {
      const weekStart = startOfWeek(currentWeek, { weekStartsOn: 1 }); // Monday
      const userId = "current-user-id"; // TODO: Get from auth context
      const data = await timeService.getTimesheet(userId, weekStart.toISOString());
      setTimesheet(data);
    } catch (error) {
      console.error("Failed to load timesheet:", error);
    } finally {
      setLoading(false);
    }
  };

  const formatDuration = (minutes: number) => {
    const hrs = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return hrs > 0 ? `${hrs}h ${mins}m` : `${mins}m`;
  };

  const navigateWeek = (direction: "prev" | "next") => {
    const days = direction === "prev" ? -7 : 7;
    setCurrentWeek(addDays(currentWeek, days));
  };

  const exportTimesheet = () => {
    if (!timesheet) return;

    const csvContent = [
      ["Date", "Total Hours", "Billable Hours", "Entries"],
      ...timesheet.dailyTotals.map((day) => [
        format(new Date(day.date), "yyyy-MM-dd"),
        (day.totalMinutes / 60).toFixed(2),
        (day.billableMinutes / 60).toFixed(2),
        day.entries.length.toString(),
      ]),
    ]
      .map((row) => row.join(","))
      .join("\n");

    const blob = new Blob([csvContent], { type: "text/csv" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `timesheet-${format(currentWeek, "yyyy-MM-dd")}.csv`;
    a.click();
  };

  if (loading) {
    return <div className="text-center py-8">Loading timesheet...</div>;
  }

  const weekStart = startOfWeek(currentWeek, { weekStartsOn: 1 });
  const weekDays = Array.from({ length: 7 }, (_, i) => addDays(weekStart, i));

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-4">
          <h2 className="text-2xl font-bold">Timesheet</h2>
          <div className="flex items-center gap-2">
            <Button variant="outline" size="sm" onClick={() => navigateWeek("prev")}>
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <span className="text-sm font-medium min-w-[200px] text-center">
              {format(weekStart, "MMM dd")} - {format(addDays(weekStart, 6), "MMM dd, yyyy")}
            </span>
            <Button variant="outline" size="sm" onClick={() => navigateWeek("next")}>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={exportTimesheet}>
            <Download className="h-4 w-4 mr-2" />
            Export
          </Button>
          <Button size="sm">Submit for Approval</Button>
        </div>
      </div>

      <div className="space-y-4">
        <div className="grid grid-cols-7 gap-2">
          {weekDays.map((day, index) => {
            const dayData = timesheet?.dailyTotals[index];
            const isToday = isSameDay(day, new Date());

            return (
              <div
                key={index}
                className={`p-4 border rounded-lg ${
                  isToday ? "border-primary bg-primary/5" : ""
                }`}
              >
                <div className="text-sm font-medium mb-2">
                  {format(day, "EEE")}
                  <br />
                  {format(day, "dd")}
                </div>
                <div className="space-y-1">
                  <div className="text-lg font-bold">
                    {formatDuration(dayData?.totalMinutes || 0)}
                  </div>
                  <div className="text-xs text-muted-foreground">
                    {dayData?.billableMinutes || 0}m billable
                  </div>
                  <div className="text-xs text-muted-foreground">
                    {dayData?.entries.length || 0} entries
                  </div>
                </div>
              </div>
            );
          })}
        </div>

        <div className="pt-4 border-t">
          <div className="flex justify-between items-center">
            <span className="text-lg font-semibold">Weekly Total</span>
            <span className="text-2xl font-bold">
              {formatDuration(timesheet?.totalMinutes || 0)}
            </span>
          </div>
          <div className="flex justify-between items-center mt-2">
            <span className="text-sm text-muted-foreground">Billable Time</span>
            <span className="text-lg font-semibold text-green-600">
              {formatDuration(
                timesheet?.dailyTotals.reduce((sum, day) => sum + day.billableMinutes, 0) || 0
              )}
            </span>
          </div>
        </div>
      </div>
    </Card>
  );
}
