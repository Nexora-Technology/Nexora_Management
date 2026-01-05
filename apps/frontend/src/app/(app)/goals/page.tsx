"use client"

import * as React from "react"
import { useRouter } from "next/navigation"
import { Target, LayoutDashboard, List, TreeDeciduous, Plus } from "lucide-react"
import { Button } from "@/components/ui/button"
import { ProgressDashboard } from "@/components/goals/progress-dashboard"
import { ObjectiveTree } from "@/components/goals/objective-tree"
import { ObjectiveCard } from "@/components/goals/objective-card"
import { goalsApi } from "@/features/goals/api"
import { ObjectiveTreeNode, Objective, ProgressDashboard as DashboardData } from "@/features/goals/types"

/**
 * Goals Page
 *
 * Main OKR tracking page with 3 views:
 * - Dashboard: Progress statistics and breakdown
 * - Tree: Hierarchical view (3 levels)
 * - List: Paginated flat list
 */
export default function GoalsPage() {
  const router = useRouter()
  const [viewMode, setViewMode] = React.useState<"dashboard" | "tree" | "list">("dashboard")
  const [isLoading, setIsLoading] = React.useState(true)
  const [error, setError] = React.useState<string | null>(null)
  const [dashboardData, setDashboardData] = React.useState<DashboardData | null>(null)
  const [treeData, setTreeData] = React.useState<ObjectiveTreeNode[]>([])
  const [listData, setListData] = React.useState<Objective[]>([])

  // Mock workspace ID - in real app, get from context/URL
  const workspaceId = "default-workspace-id"

  React.useEffect(() => {
    loadData()
  }, [workspaceId, viewMode])

  const loadData = async () => {
    setIsLoading(true)
    setError(null)
    try {
      // Load data based on view mode
      if (viewMode === "dashboard") {
        const data = await goalsApi.getProgressDashboard(workspaceId)
        setDashboardData(data)
      } else if (viewMode === "tree") {
        const data = await goalsApi.getObjectiveTree(workspaceId)
        setTreeData(data)
      } else {
        const data = await goalsApi.getObjectives(workspaceId, { page: 1, pageSize: 50 })
        setListData(data.items)
      }
    } catch (err) {
      setError("Failed to load objectives data. Please try again.")
      console.error("Error loading goals:", err)
    } finally {
      setIsLoading(false)
    }
  }

  const handleObjectiveClick = (objectiveId: string) => {
    router.push(`/goals/${objectiveId}`)
  }

  const handleCreateObjective = () => {
    // In real app, open modal to create objective
    alert("Create objective modal would open here")
  }

  return (
    <div className="h-full flex flex-col">
      {/* Header */}
      <div className="border-b border-gray-200 dark:border-gray-700 px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center">
              <Target className="w-5 h-5 text-primary" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                Goals & OKRs
              </h1>
              <p className="text-sm text-gray-500 dark:text-gray-400">
                Track objectives and key results
              </p>
            </div>
          </div>

          <div className="flex items-center gap-2">
            {/* View Mode Toggle */}
            <div className="flex items-center border border-gray-200 dark:border-gray-700 rounded-lg overflow-hidden">
              <button
                onClick={() => { setViewMode("dashboard"); loadData() }}
                className={cn(
                  "px-3 py-2 text-sm font-medium transition-colors",
                  viewMode === "dashboard"
                    ? "bg-primary text-white"
                    : "bg-white dark:bg-gray-800 text-gray-600 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-700"
                )}
              >
                <LayoutDashboard className="w-4 h-4" />
              </button>
              <button
                onClick={() => { setViewMode("tree"); loadData() }}
                className={cn(
                  "px-3 py-2 text-sm font-medium transition-colors",
                  viewMode === "tree"
                    ? "bg-primary text-white"
                    : "bg-white dark:bg-gray-800 text-gray-600 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-700"
                )}
              >
                <TreeDeciduous className="w-4 h-4" />
              </button>
              <button
                onClick={() => { setViewMode("list"); loadData() }}
                className={cn(
                  "px-3 py-2 text-sm font-medium transition-colors",
                  viewMode === "list"
                    ? "bg-primary text-white"
                    : "bg-white dark:bg-gray-800 text-gray-600 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-700"
                )}
              >
                <List className="w-4 h-4" />
              </button>
            </div>

            {/* Create Button */}
            <Button onClick={handleCreateObjective} className="gap-2">
              <Plus className="w-4 h-4" />
              New Objective
            </Button>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="flex-1 overflow-auto p-6">
        {isLoading ? (
          <div className="flex items-center justify-center h-64">
            <div className="text-gray-500 dark:text-gray-400">Loading goals...</div>
          </div>
        ) : error ? (
          <div className="flex items-center justify-center h-64">
            <div className="text-error">{error}</div>
          </div>
        ) : (
          <>
            {/* Dashboard View */}
            {viewMode === "dashboard" && dashboardData && (
              <ProgressDashboard data={dashboardData} />
            )}

            {/* Tree View */}
            {viewMode === "tree" && (
              <ObjectiveTree
                objectives={treeData}
                onObjectiveClick={handleObjectiveClick}
              />
            )}

            {/* List View */}
            {viewMode === "list" && (
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {listData.map((objective) => (
                  <ObjectiveCard
                    key={objective.id}
                    objective={objective}
                    onClick={() => handleObjectiveClick(objective.id)}
                  />
                ))}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}

function cn(...classes: (string | boolean | undefined)[]) {
  return classes.filter(Boolean).join(" ")
}
