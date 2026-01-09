"use client";

import { useState } from "react";
import { GlobalTimer } from "@/components/time/global-timer";
import { TimerHistory } from "@/components/time/timer-history";
import { TimeEntryForm } from "@/components/time/time-entry-form";
import { TimeEntry } from "@/lib/services/time-service";

export default function TimePage() {
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleTimerUpdate = (entry: TimeEntry | null) => {
    setRefreshTrigger((prev) => prev + 1);
  };

  return (
    <div className="container mx-auto py-8 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Time Tracking</h1>
          <p className="text-muted-foreground">
            Track your time and manage your timesheets
          </p>
        </div>
        <TimeEntryForm />
      </div>

      <GlobalTimer onTimerUpdate={handleTimerUpdate} />
      <TimerHistory refreshTrigger={refreshTrigger} />
    </div>
  );
}
