"use client"

import * as React from "react"
import { memo } from "react"
import { X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import {
  Dialog,
  DialogOverlay,
  DialogContent,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Task, TaskStatus, TaskPriority } from "./types"
import { cn } from "@/lib/utils"

interface TaskModalProps {
  open?: boolean
  onOpenChange?: (open: boolean) => void
  task?: Task
  onSubmit?: (task: Omit<Task, "id" | "createdAt" | "updatedAt">) => void
  mode?: "create" | "edit"
  isLoading?: boolean
  className?: string
}

const statusOptions: { value: TaskStatus; label: string }[] = [
  { value: "todo", label: "To Do" },
  { value: "inProgress", label: "In Progress" },
  { value: "complete", label: "Complete" },
  { value: "overdue", label: "Overdue" },
]

const priorityOptions: { value: TaskPriority; label: string }[] = [
  { value: "urgent", label: "Urgent" },
  { value: "high", label: "High" },
  { value: "medium", label: "Medium" },
  { value: "low", label: "Low" },
]

export const TaskModal = memo(function TaskModal({
  open,
  onOpenChange,
  task,
  onSubmit,
  mode = "create",
  isLoading = false,
  className,
}: TaskModalProps) {
  const [formData, setFormData] = React.useState({
    title: task?.title || "",
    description: task?.description || "",
    status: task?.status || "todo",
    priority: task?.priority || "medium",
    dueDate: task?.dueDate || "",
    projectId: task?.projectId || "",
    commentCount: task?.commentCount || 0,
    attachmentCount: task?.attachmentCount || 0,
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()

    if (!formData.title.trim()) return

    onSubmit?.({
      title: formData.title,
      description: formData.description || undefined,
      status: formData.status,
      priority: formData.priority,
      dueDate: formData.dueDate || undefined,
      projectId: formData.projectId || "default",
      commentCount: formData.commentCount,
      attachmentCount: formData.attachmentCount,
    })

    if (mode === "create") {
      setFormData({
        title: "",
        description: "",
        status: "todo",
        priority: "medium",
        dueDate: "",
        projectId: "",
        commentCount: 0,
        attachmentCount: 0,
      })
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogOverlay className="bg-black/50 backdrop-blur-sm" />
      <DialogContent
        className={cn(
          "bg-white dark:bg-gray-800 rounded-lg shadow-lg max-w-md w-full mx-4",
          "animate-in fade-in-0 zoom-in-95",
          className
        )}
        aria-describedby="task-modal-description"
      >
        <div className="flex items-center justify-between mb-4">
          <DialogTitle className="text-lg font-semibold text-gray-900 dark:text-white">
            {mode === "create" ? "Create Task" : "Edit Task"}
          </DialogTitle>
          <button
            onClick={() => onOpenChange?.(false)}
            className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <DialogDescription className="text-sm text-gray-500 dark:text-gray-400 mb-4">
          {mode === "create"
            ? "Fill in the details to create a new task."
            : "Update the task details below."}
        </DialogDescription>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Title */}
          <div>
            <label
              htmlFor="title"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Title <span className="text-red-500">*</span>
            </label>
            <Input
              id="title"
              type="text"
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              placeholder="Enter task title"
              className="w-full"
              required
            />
          </div>

          {/* Description */}
          <div>
            <label
              htmlFor="description"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Description
            </label>
            <Textarea
              id="description"
              value={formData.description}
              onChange={(e) =>
                setFormData({ ...formData, description: e.target.value })
              }
              placeholder="Enter task description"
              rows={4}
              className="w-full resize-none"
            />
          </div>

          {/* Status */}
          <div>
            <label
              htmlFor="status"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Status
            </label>
            <Select
              value={formData.status}
              onValueChange={(value) =>
                setFormData({ ...formData, status: value as TaskStatus })
              }
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Select status" />
              </SelectTrigger>
              <SelectContent>
                {statusOptions.map((option) => (
                  <SelectItem key={option.value} value={option.value}>
                    {option.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Priority */}
          <div>
            <label
              htmlFor="priority"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Priority
            </label>
            <Select
              value={formData.priority}
              onValueChange={(value) =>
                setFormData({ ...formData, priority: value as TaskPriority })
              }
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Select priority" />
              </SelectTrigger>
              <SelectContent>
                {priorityOptions.map((option) => (
                  <SelectItem key={option.value} value={option.value}>
                    {option.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Due Date */}
          <div>
            <label
              htmlFor="dueDate"
              className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
            >
              Due Date
            </label>
            <Input
              id="dueDate"
              type="date"
              value={formData.dueDate}
              onChange={(e) => setFormData({ ...formData, dueDate: e.target.value })}
              className="w-full"
            />
          </div>

          {/* Actions */}
          <div className="flex items-center justify-end gap-2 pt-4 border-t border-gray-200 dark:border-gray-700">
            <Button
              type="button"
              variant="secondary"
              onClick={() => onOpenChange?.(false)}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={!formData.title.trim() || isLoading}>
              {isLoading ? "Saving..." : mode === "create" ? "Create Task" : "Save Changes"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  )
})
