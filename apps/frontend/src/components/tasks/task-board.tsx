"use client"

import * as React from "react"
import { Task } from "./types"
import { TaskCard } from "./task-card"
import { BoardLayout, BoardColumn } from "@/components/layout/board-layout"

interface TaskBoardProps {
  tasks: Task[]
  onTaskClick?: (task: Task) => void
  className?: string
}

export function TaskBoard({ tasks, onTaskClick, className }: TaskBoardProps) {
  const tasksByStatus = React.useMemo(() => {
    return {
      todo: tasks.filter((t) => t.status === "todo"),
      inProgress: tasks.filter((t) => t.status === "inProgress"),
      complete: tasks.filter((t) => t.status === "complete"),
      overdue: tasks.filter((t) => t.status === "overdue"),
    }
  }, [tasks])

  const handleCardClick = (task: Task) => {
    onTaskClick?.(task)
  }

  return (
    <BoardLayout className={className}>
      <BoardColumn title="To Do" count={tasksByStatus.todo.length}>
        {tasksByStatus.todo.map((task) => (
          <TaskCard key={task.id} task={task} onClick={() => handleCardClick(task)} />
        ))}
      </BoardColumn>

      <BoardColumn title="In Progress" count={tasksByStatus.inProgress.length}>
        {tasksByStatus.inProgress.map((task) => (
          <TaskCard key={task.id} task={task} onClick={() => handleCardClick(task)} />
        ))}
      </BoardColumn>

      <BoardColumn title="Complete" count={tasksByStatus.complete.length}>
        {tasksByStatus.complete.map((task) => (
          <TaskCard key={task.id} task={task} onClick={() => handleCardClick(task)} />
        ))}
      </BoardColumn>

      <BoardColumn title="Overdue" count={tasksByStatus.overdue.length}>
        {tasksByStatus.overdue.map((task) => (
          <TaskCard key={task.id} task={task} onClick={() => handleCardClick(task)} />
        ))}
      </BoardColumn>
    </BoardLayout>
  )
}
