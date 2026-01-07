"use client"

import * as React from "react"
import { ArrowLeft, Calendar, User } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Container } from "@/components/layout"
import { Breadcrumb } from "@/components/layout"
import Link from "next/link"
import { mockTasks } from "@/components/tasks"
import { useParams } from "next/navigation"
import { STATUS_LABELS, STATUS_BADGE_VARIANTS } from "@/components/tasks/constants"

export default function TaskDetailPage() {
  const params = useParams()
  const task = mockTasks.find((t) => t.id === params.id)

  if (!task) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="text-center">
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">
            Task not found
          </h2>
          <Link href={"/spaces" as any}>
            <Button variant="secondary">Back to Spaces</Button>
          </Link>
        </div>
      </div>
    )
  }

  return (
    <Container size="md" className="py-6">
      {/* Breadcrumb */}
      <Breadcrumb
        items={[
          { label: "Home", href: "/" },
          { label: "Spaces", href: "/spaces" },
          // TODO: Add space, folder, and list names when available from API
          // { label: task.spaceName, href: `/spaces/${task.spaceId}` },
          // ...(task.folderName ? [{ label: task.folderName, href: `/spaces/${task.spaceId}/folders/${task.folderId}` }] : []),
          // { label: task.listName, href: `/lists/${task.listId}` },
          { label: task.title },
        ].filter(Boolean)}
        className="mb-6"
      />

      {/* Header */}
      <div className="mb-6">
        <Link href={"/spaces" as any}>
          <Button variant="ghost" size="sm" className="mb-4 gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>

        <div className="flex items-start justify-between">
          <div className="flex-1">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
              {task.title}
            </h1>
            <div className="flex items-center gap-3">
              <Badge status={STATUS_BADGE_VARIANTS[task.status]}>
                {STATUS_LABELS[task.status]}
              </Badge>
              <span className="text-sm text-gray-500 dark:text-gray-400">
                Priority: {task.priority}
              </span>
            </div>
          </div>

          <div className="flex gap-2">
            <Button variant="secondary">Edit</Button>
            <Button variant="destructive">Delete</Button>
          </div>
        </div>
      </div>

      {/* Description */}
      {task.description && (
        <div className="mb-6">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-3">
            Description
          </h2>
          <p className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">
            {task.description}
          </p>
        </div>
      )}

      {/* Meta */}
      <div className="grid grid-cols-2 gap-4 border-t border-gray-200 dark:border-gray-700 pt-4">
        {task.assignee && (
          <div className="flex items-center gap-3">
            <User className="h-5 w-5 text-gray-400" />
            <div>
              <p className="text-xs text-gray-500 dark:text-gray-400">Assignee</p>
              <div className="flex items-center gap-2">
                <Avatar className="h-6 w-6">
                  {task.assignee.avatar && (
                    <AvatarImage src={task.assignee.avatar} alt={task.assignee.name} />
                  )}
                  <AvatarFallback name={task.assignee.name} />
                </Avatar>
                <span className="text-sm text-gray-900 dark:text-white">
                  {task.assignee.name}
                </span>
              </div>
            </div>
          </div>
        )}

        {task.dueDate && (
          <div className="flex items-center gap-3">
            <Calendar className="h-5 w-5 text-gray-400" />
            <div>
              <p className="text-xs text-gray-500 dark:text-gray-400">Due Date</p>
              <p className="text-sm text-gray-900 dark:text-white">
                {new Date(task.dueDate).toLocaleDateString()}
              </p>
            </div>
          </div>
        )}
      </div>
    </Container>
  )
}
