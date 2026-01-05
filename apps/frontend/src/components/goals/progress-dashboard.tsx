"use client"

import * as React from "react"
import { Target, TrendingUp, AlertCircle, CheckCircle2 } from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Progress } from "@/components/ui/progress"
import { cn } from "@/lib/utils"
import { ProgressDashboard as DashboardData } from "@/features/goals/types"

interface ProgressDashboardProps {
  data: DashboardData
  className?: string
}

/**
 * ProgressDashboard Component
 *
 * Displays OKR progress statistics including:
 * - Overall progress metrics
 * - Status breakdown chart
 * - Top and bottom performing objectives
 */
export function ProgressDashboard({ data, className }: ProgressDashboardProps) {
  const statusColors = {
    "on-track": "bg-success",
    "at-risk": "bg-warning",
    "off-track": "bg-error",
    "completed": "bg-primary"
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case "on-track": return TrendingUp
      case "completed": return CheckCircle2
      default: return AlertCircle
    }
  }

  return (
    <div className={cn("grid gap-4 md:grid-cols-2 lg:grid-cols-4", className)}>
      {/* Total Objectives */}
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="text-sm font-medium text-gray-600 dark:text-gray-400">
            Total Objectives
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-end gap-2">
            <div className="text-3xl font-bold text-gray-900 dark:text-white">
              {data.totalObjectives}
            </div>
            <Target className="w-5 h-5 text-primary mb-1" />
          </div>
        </CardContent>
      </Card>

      {/* Average Progress */}
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="text-sm font-medium text-gray-600 dark:text-gray-400">
            Avg Progress
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            <div className="text-3xl font-bold text-gray-900 dark:text-white">
              {data.averageProgress}%
            </div>
            <Progress
              value={data.averageProgress}
              className="h-2"
              indicatorClassName={
                data.averageProgress >= 80 ? "bg-success" :
                data.averageProgress >= 50 ? "bg-warning" :
                "bg-error"
              }
            />
          </div>
        </CardContent>
      </Card>

      {/* Total Key Results */}
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="text-sm font-medium text-gray-600 dark:text-gray-400">
            Key Results
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-between">
            <div>
              <div className="text-3xl font-bold text-gray-900 dark:text-white">
                {data.totalKeyResults}
              </div>
              <div className="text-xs text-gray-500 mt-1">
                {data.completedKeyResults} completed
              </div>
            </div>
            <div className="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center">
              <Target className="w-6 h-6 text-primary" />
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Completion Rate */}
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="text-sm font-medium text-gray-600 dark:text-gray-400">
            Completion Rate
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="text-3xl font-bold text-gray-900 dark:text-white">
            {data.totalKeyResults > 0
              ? Math.round((data.completedKeyResults / data.totalKeyResults) * 100)
              : 0}%
          </div>
          <Progress
            value={data.totalKeyResults > 0
              ? (data.completedKeyResults / data.totalKeyResults) * 100
              : 0
            }
            className="h-2 mt-2"
            indicatorClassName="bg-success"
          />
        </CardContent>
      </Card>

      {/* Status Breakdown */}
      <Card className="md:col-span-2">
        <CardHeader>
          <CardTitle className="text-base">Status Breakdown</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {data.statusBreakdown.map((status) => {
              const Icon = getStatusIcon(status.status)
              return (
                <div key={status.status} className="flex items-center gap-3">
                  <Icon className={cn(
                    "w-5 h-5",
                    status.status === "on-track" ? "text-success" :
                    status.status === "completed" ? "text-primary" :
                    status.status === "at-risk" ? "text-warning" :
                    "text-error"
                  )} />
                  <div className="flex-1">
                    <div className="flex items-center justify-between text-sm mb-1">
                      <span className="capitalize font-medium">
                        {status.status.replace("-", " ")}
                      </span>
                      <span className="text-gray-600 dark:text-gray-400">
                        {status.count} ({status.percentage.toFixed(1)}%)
                      </span>
                    </div>
                    <Progress
                      value={status.percentage}
                      className="h-2"
                      indicatorClassName={statusColors[status.status as keyof typeof statusColors]}
                    />
                  </div>
                </div>
              )
            })}
          </div>
        </CardContent>
      </Card>

      {/* Top Performers */}
      <Card className="md:col-span-2">
        <CardHeader>
          <CardTitle className="text-base flex items-center gap-2">
            <TrendingUp className="w-5 h-5 text-success" />
            Top Performing Objectives
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            {data.topObjectives.map((objective) => (
              <div
                key={objective.id}
                className="flex items-center justify-between p-2 rounded-lg bg-gray-50 dark:bg-gray-800/50"
              >
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-900 dark:text-white truncate">
                    {objective.title}
                  </p>
                  <p className="text-xs text-gray-500">
                    {objective.keyResultCount} KR{objective.keyResultCount !== 1 ? "s" : ""}
                  </p>
                </div>
                <div className="text-right">
                  <div className="text-sm font-semibold text-success">
                    {objective.progress}%
                  </div>
                  <div
                    className={cn(
                      "text-xs capitalize",
                      objective.status === "on-track" ? "text-success" :
                      objective.status === "at-risk" ? "text-warning" :
                      "text-error"
                    )}
                  >
                    {objective.status.replace("-", " ")}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
