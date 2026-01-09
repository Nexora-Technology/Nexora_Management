"use client";

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { timeService, TimeEntry } from "@/lib/services/time-service";
import { format } from "date-fns";
import { Trash2, Edit } from "lucide-react";

interface TimerHistoryProps {
  refreshTrigger?: number;
}

export function TimerHistory({ refreshTrigger }: TimerHistoryProps) {
  const [entries, setEntries] = useState<TimeEntry[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadEntries();
  }, [refreshTrigger]);

  const loadEntries = async () => {
    setLoading(true);
    try {
      const result = await timeService.getTimeEntries({
        page: 1,
        pageSize: 10,
      });
      setEntries(result.data);
    } catch (error) {
      console.error("Failed to load time entries:", error);
    } finally {
      setLoading(false);
    }
  };

  const formatDuration = (minutes: number) => {
    const hrs = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return hrs > 0 ? `${hrs}h ${mins}m` : `${mins}m`;
  };

  const getBadgeVariant = (status: string) => {
    switch (status) {
      case "approved":
        return "default";
      case "rejected":
        return "destructive";
      case "submitted":
        return "secondary";
      default:
        return "outline";
    }
  };

  if (loading) {
    return <div className="text-center py-8">Loading...</div>;
  }

  return (
    <Card className="p-6">
      <h3 className="text-lg font-semibold mb-4">Recent Time Entries</h3>
      <div className="space-y-3">
        {entries.length === 0 ? (
          <p className="text-sm text-muted-foreground text-center py-4">
            No time entries yet
          </p>
        ) : (
          entries.map((entry) => (
            <div
              key={entry.id}
              className="flex items-center justify-between p-3 border rounded-lg hover:bg-muted/50"
            >
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <span className="font-medium">{entry.description || "No description"}</span>
                  <span
                    className={`text-xs px-2 py-0.5 rounded-full ${getBadgeVariant(
                      entry.status
                    )}`}
                  >
                    {entry.status}
                  </span>
                  {entry.isBillable && (
                    <span className="text-xs text-green-600">💰 Billable</span>
                  )}
                </div>
                <div className="flex items-center gap-4 text-sm text-muted-foreground">
                  <span>{formatDuration(entry.durationMinutes)}</span>
                  <span>
                    {format(new Date(entry.startTime), "MMM dd, HH:mm")}
                  </span>
                  {entry.taskId && <span>Task: {entry.taskId.slice(0, 8)}</span>}
                </div>
              </div>
              <div className="flex items-center gap-2">
                <Button variant="ghost" size="sm">
                  <Edit className="h-4 w-4" />
                </Button>
                <Button variant="ghost" size="sm">
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            </div>
          ))
        )}
      </div>
    </Card>
  );
}
