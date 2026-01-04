"use client"

import * as React from "react"
import { memo, useCallback, useMemo } from "react"
import { Task } from "./types"
import { TaskCard } from "./task-card"
import { BoardLayout, BoardColumn } from "@/components/layout/board-layout"

interface TaskBoardProps {
  tasks: Task[]
  onTaskClick?: (task: Task) => void
  className?: string
}

export const TaskBoard = memo(function TaskBoard({ tasks, onTaskClick, className }: TaskBoardProps) {
  const tasksByStatus = React.useMemo(() => {
    const result: { todo: Task[]; inProgress: Task[]; complete: Task[]; overdue: Task[] } = {
      todo: [],
      inProgress: [],
      complete: [],
      overdue: [],
    }
    for (const task of tasks) {
      if (task.status === "todo") result.todo.push(task)
      else if (task.status === "inProgress") result.inProgress.push(task)
      else if (task.status === "complete") result.complete.push(task)
      else if (task.status === "overdue") result.overdue.push(task)
    }
    return result
  }, [tasks])

  const handleCardClick = useCallback((task: Task) => {
    onTaskClick?.(task)
  }, [onTaskClick])

  const createClickHandler = useCallback((task: Task) => () => handleCardClick(task), [handleCardClick])

  const totalTasks = useMemo(() => tasks.length, [tasks])

  return (
    <BoardLayout className={className}>
      <div aria-live="polite" aria-atomic="true" className="sr-only">
        {totalTasks} tasks loaded
      </div>
      <BoardColumn title="To Do" count={tasksByStatus.todo.length}>
        {tasksByStatus.todo.map((task) => (
          <TaskCard key={task.id} task={task} onClick={createClickHandler(task)} />
        ))}
      </BoardColumn>

      <BoardColumn title="In Progress" count={tasksByStatus.inProgress.length}>
        {tasksByStatus.inProgress.map((task) => (
          <TaskCard key={task.id} task={task} onClick={createClickHandler(task)} />
        ))}
      </BoardColumn>

      <BoardColumn title="Complete" count={tasksByStatus.complete.length}>
        {tasksByStatus.complete.map((task) => (
          <TaskCard key={task.id} task={task} onClick={createClickHandler(task)} />
        ))}
      </BoardColumn>

      <BoardColumn title="Overdue" count={tasksByStatus.overdue.length}>
        {tasksByStatus.overdue.map((task) => (
          <TaskCard key={task.id} task={task} onClick={createClickHandler(task)} />
        ))}
      </BoardColumn>
    </BoardLayout>
  )
}, (prevProps, nextProps) => {
  return (
    prevProps.tasks.length === nextProps.tasks.length &&
    prevProps.tasks.every((t, i) => t.id === nextProps.tasks[i]?.id) &&
    prevProps.tasks.every((t, i) => t.status === nextProps.tasks[i]?.status) &&
    prevProps.onTaskClick === nextProps.onTaskClick &&
    prevProps.className === nextProps.className
  )
})
