"use client";

import { useState, useEffect } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { timeService, TimeReport, TaskTimeBreakdown } from "@/lib/services/time-service";
import { format, subDays, startOfMonth, endOfMonth } from "date-fns";
import { BarChart3, Download, TrendingUp } from "lucide-react";

export function TimeReports() {
  const [period, setPeriod] = useState<"week" | "month" | "custom">("week");
  const [startDate, setStartDate] = useState(format(subDays(new Date(), 7), "yyyy-MM-dd"));
  const [endDate, setEndDate] = useState(format(new Date(), "yyyy-MM-dd"));
  const [report, setReport] = useState<TimeReport | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadReport();
  }, [period]);

  const loadReport = async () => {
    setLoading(true);
    try {
      let start = new Date();
      let end = new Date();

      if (period === "week") {
        start = subDays(new Date(), 7);
        end = new Date();
      } else if (period === "month") {
        start = startOfMonth(new Date());
        end = endOfMonth(new Date());
      } else {
        start = new Date(startDate);
        end = new Date(endDate);
      }

      const userId = "current-user-id"; // TODO: Get from auth context
      const data = await timeService.getTimeReport(
        userId,
        start.toISOString(),
        end.toISOString()
      );
      setReport(data);
    } catch (error) {
      console.error("Failed to load time report:", error);
    } finally {
      setLoading(false);
    }
  };

  const formatDuration = (minutes: number) => {
    const hrs = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return hrs > 0 ? `${hrs}h ${mins}m` : `${mins}m`;
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  const exportReport = () => {
    if (!report) return;

    const csvContent = [
      ["Task", "Hours", "Entries"],
      ...report.taskBreakdown.map((task) => [
        task.taskTitle || "No task",
        (task.totalMinutes / 60).toFixed(2),
        task.entryCount.toString(),
      ]),
      ["Total", (report.totalMinutes / 60).toFixed(2), ""],
      ["Billable", (report.billableMinutes / 60).toFixed(2), ""],
      ["Amount", formatCurrency(report.totalAmount), ""],
    ]
      .map((row) => row.join(","))
      .join("\n");

    const blob = new Blob([csvContent], { type: "text/csv" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `time-report-${format(new Date(), "yyyy-MM-dd")}.csv`;
    a.click();
  };

  return (
    <div className="space-y-6">
      <Card className="p-6">
        <h2 className="text-2xl font-bold mb-4">Time Reports</h2>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="space-y-2">
            <Label>Period</Label>
            <Select value={period} onValueChange={(v: "week" | "month" | "custom") => setPeriod(v)}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="week">Last 7 Days</SelectItem>
                <SelectItem value="month">This Month</SelectItem>
                <SelectItem value="custom">Custom</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {period === "custom" && (
            <>
              <div className="space-y-2">
                <Label>Start Date</Label>
                <Input
                  type="date"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label>End Date</Label>
                <Input
                  type="date"
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                />
              </div>
            </>
          )}

          <div className="flex items-end">
            <Button onClick={loadReport} disabled={loading} className="w-full">
              {loading ? "Loading..." : "Generate Report"}
            </Button>
          </div>
        </div>

        {report && (
          <>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
              <Card className="p-4">
                <div className="flex items-center gap-2 mb-2">
                  <TrendingUp className="h-5 w-5 text-blue-500" />
                  <span className="text-sm font-medium">Total Time</span>
                </div>
                <div className="text-2xl font-bold">
                  {formatDuration(report.totalMinutes)}
                </div>
                <div className="text-xs text-muted-foreground mt-1">
                  {(report.totalMinutes / 60).toFixed(1)} hours
                </div>
              </Card>

              <Card className="p-4">
                <div className="flex items-center gap-2 mb-2">
                  <BarChart3 className="h-5 w-5 text-green-500" />
                  <span className="text-sm font-medium">Billable Time</span>
                </div>
                <div className="text-2xl font-bold text-green-600">
                  {formatDuration(report.billableMinutes)}
                </div>
                <div className="text-xs text-muted-foreground mt-1">
                  {report.totalMinutes > 0
                    ? `${((report.billableMinutes / report.totalMinutes) * 100).toFixed(0)}%`
                    : "0%"}
                  of total
                </div>
              </Card>

              <Card className="p-4">
                <div className="flex items-center gap-2 mb-2">
                  <Download className="h-5 w-5 text-purple-500" />
                  <span className="text-sm font-medium">Billable Amount</span>
                </div>
                <div className="text-2xl font-bold text-purple-600">
                  {formatCurrency(report.totalAmount)}
                </div>
                <div className="text-xs text-muted-foreground mt-1">
                  @ $50/hour (default rate)
                </div>
              </Card>
            </div>

            <div className="flex justify-end mb-4">
              <Button variant="outline" size="sm" onClick={exportReport}>
                <Download className="h-4 w-4 mr-2" />
                Export CSV
              </Button>
            </div>

            <Card className="p-4">
              <h3 className="text-lg font-semibold mb-4">Time by Task</h3>
              <div className="space-y-3">
                {report.taskBreakdown.map((task, index) => (
                  <div key={index} className="flex items-center justify-between p-3 border rounded-lg">
                    <div className="flex-1">
                      <div className="font-medium">{task.taskTitle || "No task"}</div>
                      <div className="text-sm text-muted-foreground">
                        {task.entryCount} entries
                      </div>
                    </div>
                    <div className="text-right">
                      <div className="font-semibold">{formatDuration(task.totalMinutes)}</div>
                      <div className="text-xs text-muted-foreground">
                        {(task.totalMinutes / 60).toFixed(1)}h
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </Card>
          </>
        )}
      </Card>
    </div>
  );
}
