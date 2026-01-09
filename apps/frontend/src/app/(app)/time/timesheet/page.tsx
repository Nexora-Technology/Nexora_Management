"use client";

import { TimesheetView } from "@/components/time/timesheet-view";

export default function TimesheetPage() {
  return (
    <div className="container mx-auto py-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold">Weekly Timesheet</h1>
        <p className="text-muted-foreground">
          Review and submit your weekly timesheet
        </p>
      </div>

      <TimesheetView />
    </div>
  );
}
