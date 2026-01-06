"use client"

import * as React from "react"
import { TaskBoard, TaskModal, TaskToolbar, ViewMode } from "@/components/tasks"
import { mockTasks } from "@/components/tasks"
import { Task } from "@/components/tasks"
import { useRouter } from "next/navigation"

export default function BoardPage() {
  const router = useRouter()
  const [viewMode, setViewMode] = React.useState<ViewMode>("board")
  const [isModalOpen, setIsModalOpen] = React.useState(false)
  const [editingTask, setEditingTask] = React.useState<Task | undefined>()
  const [isLoading, setIsLoading] = React.useState(false)

  const handleAddTask = () => {
    setEditingTask(undefined)
    setIsModalOpen(true)
  }

  const handleEditTask = (task: Task) => {
    setEditingTask(task)
    setIsModalOpen(true)
  }

  const handleSubmitTask = async (taskData: Omit<Task, "id" | "createdAt" | "updatedAt">) => {
    setIsLoading(true)
    // TODO: Integrate with API to create/update task
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500))
    setIsLoading(false)
    setIsModalOpen(false)
  }

  const handleViewChange = (mode: ViewMode) => {
    setViewMode(mode)
    if (mode === "list") {
      router.push("/tasks")
    }
  }

  return (
    <div className="h-full flex flex-col">
      {/* Header */}
      <div className="border-b border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 px-6 py-4">
        <h1 className="text-2xl font-semibold text-gray-900 dark:text-white">
          Tasks
        </h1>
      </div>

      {/* Content */}
      <div className="flex-1 overflow-auto p-6">
        <TaskToolbar
          onAddTask={handleAddTask}
          viewMode={viewMode}
          onViewModeChange={handleViewChange}
        />

        {viewMode === "board" && (
          <TaskBoard tasks={mockTasks} onTaskClick={handleEditTask} />
        )}
      </div>

      {/* Task Modal */}
      <TaskModal
        open={isModalOpen}
        onOpenChange={setIsModalOpen}
        task={editingTask}
        onSubmit={handleSubmitTask}
        mode={editingTask ? "edit" : "create"}
        isLoading={isLoading}
      />
    </div>
  )
}
