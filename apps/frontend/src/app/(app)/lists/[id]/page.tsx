"use client"

import * as React from "react"
import { useQuery } from "@tanstack/react-query"
import { useParams, useRouter } from "next/navigation"
import { spacesApi } from "@/features/spaces/api"
import type { TaskList } from "@/features/spaces/types"
import { Breadcrumb } from "@/components/layout/breadcrumb"

export default function ListDetailPage() {
  const params = useParams()
  const router = useRouter()
  const listId = params.id as string

  const { data: list, isLoading: listLoading } = useQuery({
    queryKey: ["tasklists", listId],
    queryFn: async () => {
      const response = await spacesApi.getTaskListById(listId)
      return response.data
    },
  })

  const { data: tasks, isLoading: tasksLoading } = useQuery({
    queryKey: ["tasks", listId],
    queryFn: async () => {
      // TODO: Reuse existing task API with projectId = listId
      const response = await fetch(`/api/tasks?projectId=${listId}`)
      if (!response.ok) throw new Error("Failed to fetch tasks")
      return response.json()
    },
    enabled: !!listId,
  })

  // Build breadcrumb path
  const breadcrumbItems = React.useMemo(() => {
    if (!list) return []

    const items = [
      { label: "Home", href: "/" as const },
      { label: "Spaces", href: "/spaces" as const },
    ]

    // TODO: Add space and folder names when available from API
    // items.push({ label: list.spaceName, href: `/spaces/${list.spaceId}` })
    // if (list.folderId) {
    //   items.push({ label: list.folderName, href: `/spaces/${list.spaceId}/folders/${list.folderId}` })
    // }
    items.push({ label: list.name, href: `/lists/${list.id}` as any })

    return items
  }, [list])

  if (listLoading) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="text-gray-500 dark:text-gray-400">Loading list...</div>
      </div>
    )
  }

  if (!list) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mb-2">
            List not found
          </h2>
          <button
            onClick={() => router.push("/spaces" as any)}
            className="text-primary hover:underline"
          >
            Return to Spaces
          </button>
        </div>
      </div>
    )
  }

  return (
    <div className="h-full flex flex-col bg-white dark:bg-gray-800">
      {/* Header with breadcrumb */}
      <div className="border-b border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
        <div className="p-4">
          <Breadcrumb items={breadcrumbItems} className="mb-4" />
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
                  {list.name}
                </h1>
                <span
                  className="px-2 py-1 text-xs font-medium rounded-full"
                  style={{
                    backgroundColor: list.color
                      ? `${list.color}20`
                      : "#6b728020",
                    color: list.color || "#6b7280",
                  }}
                >
                  {list.listType || "task"}
                </span>
              </div>
              {list.description && (
                <p className="text-gray-600 dark:text-gray-400">
                  {list.description}
                </p>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Toolbar */}
      <div className="border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
        <div className="px-4 py-3">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              {/* TODO: Add view toggle (board/list/calendar) */}
              <button className="px-3 py-1.5 text-sm font-medium rounded-md bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600">
                Board
              </button>
              <button className="px-3 py-1.5 text-sm font-medium rounded-md text-gray-600 dark:text-gray-400 hover:bg-gray-200 dark:hover:bg-gray-700">
                List
              </button>
            </div>

            <div className="flex items-center gap-2">
              <button
                className="px-4 py-1.5 text-sm font-medium rounded-md bg-primary text-white hover:bg-primary/90"
                onClick={() => console.log("Add task")}
              >
                Add Task
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Task Board */}
      <div className="flex-1 overflow-auto p-6">
        {tasksLoading ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-gray-500 dark:text-gray-400">
              Loading tasks...
            </div>
          </div>
        ) : tasks && tasks.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {/* TODO: Implement task board columns by status */}
            {tasks.map((task: any) => (
              <div
                key={task.id}
                className="p-4 bg-gray-50 dark:bg-gray-900 rounded-lg border border-gray-200 dark:border-gray-700"
              >
                <h3 className="font-medium text-gray-900 dark:text-gray-100">
                  {task.title}
                </h3>
                <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                  {task.description}
                </p>
              </div>
            ))}
          </div>
        ) : (
          <div className="flex flex-col items-center justify-center h-full text-center">
            <div className="mb-4">
              <svg
                className="w-16 h-16 text-gray-300 dark:text-gray-600 mx-auto"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
                />
              </svg>
            </div>
            <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100 mb-2">
              No tasks yet
            </h3>
            <p className="text-gray-500 dark:text-gray-400 mb-4">
              Create your first task to get started
            </p>
            <button
              className="px-4 py-2 text-sm font-medium rounded-md bg-primary text-white hover:bg-primary/90"
              onClick={() => console.log("Add task")}
            >
              Add Task
            </button>
          </div>
        )}
      </div>
    </div>
  )
}
