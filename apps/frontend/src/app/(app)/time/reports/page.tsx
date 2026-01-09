"use client";

import { TimeReports } from "@/components/time/time-reports";

export default function TimeReportsPage() {
  return (
    <div className="container mx-auto py-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold">Time Reports</h1>
        <p className="text-muted-foreground">
          Analyze your time tracking data and generate reports
        </p>
      </div>

      <TimeReports />
    </div>
  );
}
