/**
 * TaskBoard Drag and Drop Usage Examples
 *
 * This file provides practical examples for using the TaskBoard
 * component with drag and drop functionality.
 */

import { useState } from "react"
import { TaskBoard } from "./task-board"
import { Task } from "./types"

/**
 * Example 1: Basic Usage with Status Updates
 *
 * This example shows the most common use case - handling status changes
 * when tasks are moved between columns.
 */
export function BasicTaskBoardExample() {
  const [tasks, setTasks] = useState<Task[]>([
    {
      id: "1",
      title: "Design new landing page",
      status: "todo",
      priority: "high",
      taskType: "story",
      commentCount: 3,
      attachmentCount: 1,
      projectId: "proj-1",
      createdAt: "2024-01-01",
      updatedAt: "2024-01-01",
    },
    {
      id: "2",
      title: "Fix login bug",
      status: "inProgress",
      priority: "urgent",
      taskType: "subtask",
      commentCount: 5,
      attachmentCount: 0,
      projectId: "proj-1",
      createdAt: "2024-01-01",
      updatedAt: "2024-01-02",
    },
  ])

  const handleTaskClick = (task: Task) => {
    console.log("Task clicked:", task)
    // Open task detail modal or navigate to task page
  }

  const handleStatusChange = async (taskId: string, newStatus: Task["status"]) => {
    // Update local state immediately (optimistic update)
    setTasks((prev) =>
      prev.map((task) =>
        task.id === taskId ? { ...task, status: newStatus } : task
      )
    )

    // Sync with backend
    try {
      const response = await fetch(`/api/tasks/${taskId}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ status: newStatus }),
      })

      if (!response.ok) {
        throw new Error("Failed to update status")
      }
    } catch (error) {
      console.error("Error updating task status:", error)
      // Revert on error
      setTasks((prev) =>
        prev.map((task) =>
          task.id === taskId ? { ...task, status: task.status } : task
        )
      )
    }
  }

  const handleReorder = async (newTasks: Task[]) => {
    // Update local state
    setTasks(newTasks)

    // Sync with backend (if needed)
    try {
      await fetch("/api/tasks/reorder", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          taskIds: newTasks.map((t) => t.id),
        }),
      })
    } catch (error) {
      console.error("Error reordering tasks:", error)
    }
  }

  return (
    <TaskBoard
      tasks={tasks}
      onTaskClick={handleTaskClick}
      onTaskStatusChange={handleStatusChange}
      onTaskReorder={handleReorder}
    />
  )
}

/**
 * Example 2: Read-Only Board
 *
 * Use this when you want to display tasks without allowing drag operations.
 * You can still click on tasks to view details.
 */
export function ReadOnlyTaskBoardExample() {
  const tasks: Task[] = [
    // ... your tasks
  ]

  const handleTaskClick = (task: Task) => {
    console.log("View task:", task)
    // Open in read-only mode
  }

  return (
    <TaskBoard
      tasks={tasks}
      onTaskClick={handleTaskClick}
      // No onTaskStatusChange or onTaskReorder - board is read-only
    />
  )
}

/**
 * Example 3: with Custom Status Change Handler
 *
 * This example shows how to add custom logic when status changes,
 * such as validation, notifications, or side effects.
 */
export function TaskBoardWithValidationExample() {
  const [tasks, setTasks] = useState<Task[]>([
    // ... your tasks
  ])

  const handleStatusChange = (taskId: string, newStatus: Task["status"]) => {
    const task = tasks.find((t) => t.id === taskId)

    if (!task) return

    // Example: Prevent moving urgent tasks to Complete
    if (task.priority === "urgent" && newStatus === "complete") {
      alert("Cannot complete urgent task without review!")
      return
    }

    // Example: Show notification
    console.log(`Moving "${task.title}" to ${newStatus}`)
    // toast.success(`Task moved to ${newStatus}`)

    // Update state
    setTasks((prev) =>
      prev.map((t) =>
        t.id === taskId ? { ...t, status: newStatus } : t
      )
    )

    // Sync with backend
    updateTaskStatus(taskId, newStatus)
  }

  const updateTaskStatus = async (taskId: string, status: Task["status"]) => {
    // Your API call here
  }

  return (
    <TaskBoard
      tasks={tasks}
      onTaskStatusChange={handleStatusChange}
      onTaskReorder={(newTasks) => setTasks(newTasks)}
    />
  )
}

/**
 * Example 4: with Task Detail Modal
 *
 * This example shows how to integrate with a task detail modal
 * that opens when a task is clicked.
 */
export function TaskBoardWithModalExample() {
  const [tasks, setTasks] = useState<Task[]>([
    // ... your tasks
  ])
  const [selectedTask, setSelectedTask] = useState<Task | null>(null)
  const [isModalOpen, setIsModalOpen] = useState(false)

  const handleTaskClick = (task: Task) => {
    setSelectedTask(task)
    setIsModalOpen(true)
  }

  const handleModalClose = () => {
    setIsModalOpen(false)
    setSelectedTask(null)
  }

  return (
    <>
      <TaskBoard
        tasks={tasks}
        onTaskClick={handleTaskClick}
        onTaskStatusChange={(taskId, newStatus) => {
          setTasks((prev) =>
            prev.map((t) =>
              t.id === taskId ? { ...t, status: newStatus } : t
            )
          )
        }}
        onTaskReorder={setTasks}
      />

      {/* Task Detail Modal */}
      {isModalOpen && selectedTask && (
        <TaskDetailModal
          task={selectedTask}
          onClose={handleModalClose}
          onUpdate={(updatedTask) => {
            setTasks((prev) =>
              prev.map((t) =>
                t.id === updatedTask.id ? updatedTask : t
              )
            )
          }}
        />
      )}
    </>
  )
}

// Mock component for example
function TaskDetailModal({
  task,
  onClose,
  onUpdate,
}: {
  task: Task
  onClose: () => void
  onUpdate: (task: Task) => void
}) {
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center">
      <div className="bg-white rounded-lg p-6 max-w-md w-full">
        <h2 className="text-xl font-bold mb-4">{task.title}</h2>
        <p className="text-gray-600 mb-4">Status: {task.status}</p>
        <button
          onClick={onClose}
          className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
        >
          Close
        </button>
      </div>
    </div>
  )
}

/**
 * Example 5: with Error Handling
 *
 * This example shows proper error handling for status changes
 * with user feedback.
 */
export function TaskBoardWithErrorHandlingExample() {
  const [tasks, setTasks] = useState<Task[]>([
    // ... your tasks
  ])
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const handleStatusChange = async (taskId: string, newStatus: Task["status"]) => {
    setError(null)
    setLoading(true)

    // Optimistic update
    const oldTasks = [...tasks]
    setTasks((prev) =>
      prev.map((t) =>
        t.id === taskId ? { ...t, status: newStatus } : t
      )
    )

    try {
      const response = await fetch(`/api/tasks/${taskId}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ status: newStatus }),
      })

      if (!response.ok) {
        throw new Error("Failed to update task status")
      }

      // Success!
      // toast.success("Task updated successfully")
    } catch (err) {
      // Revert on error
      setTasks(oldTasks)
      setError(err instanceof Error ? err.message : "An error occurred")
      // toast.error("Failed to update task")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}
      {loading && (
        <div className="bg-blue-100 border border-blue-400 text-blue-700 px-4 py-3 rounded mb-4">
          Updating task...
        </div>
      )}
      <TaskBoard
        tasks={tasks}
        onTaskStatusChange={handleStatusChange}
        onTaskReorder={setTasks}
      />
    </div>
  )
}

/**
 * Example 6: Keyboard Navigation Tips
 *
 * For users who prefer keyboard navigation:
 *
 * 1. Tab to the drag handle (grip icon)
 * 2. Press Space or Enter to activate drag mode
 * 3. Use arrow keys to move between columns
 * 4. Press Space or Enter to drop the task
 * 5. Press Escape to cancel the drag
 *
 * Visual feedback:
 * - The drag handle is visible on hover (desktop)
 * - Focus ring appears when tabbing to the handle
 * - Screen reader announces "Drag task: [task name]"
 */
