"use client"

import * as React from "react"
import { Search, Plus, List, Grid } from "lucide-react"
import { cn } from "@/lib/utils"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"

interface TaskToolbarProps {
  onAddTask?: () => void
  viewMode?: "list" | "board"
  onViewModeChange?: (mode: "list" | "board") => void
  className?: string
}

export function TaskToolbar({
  onAddTask,
  viewMode = "list",
  onViewModeChange,
  className,
}: TaskToolbarProps) {
  return (
    <div className={cn("flex items-center justify-between gap-4 mb-6", className)}>
      {/* Left Section */}
      <div className="flex items-center gap-4 flex-1">
        {/* Search */}
        <div className="relative w-64">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
          <Input type="search" placeholder="Search tasks..." className="pl-9" />
        </div>

        {/* Status Filter */}
        <Select defaultValue="all">
          <SelectTrigger className="w-[140px]">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Status</SelectItem>
            <SelectItem value="todo">To Do</SelectItem>
            <SelectItem value="inProgress">In Progress</SelectItem>
            <SelectItem value="complete">Complete</SelectItem>
            <SelectItem value="overdue">Overdue</SelectItem>
          </SelectContent>
        </Select>

        {/* Priority Filter */}
        <Select defaultValue="all">
          <SelectTrigger className="w-[140px]">
            <SelectValue placeholder="Priority" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Priority</SelectItem>
            <SelectItem value="urgent">Urgent</SelectItem>
            <SelectItem value="high">High</SelectItem>
            <SelectItem value="medium">Medium</SelectItem>
            <SelectItem value="low">Low</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Right Section */}
      <div className="flex items-center gap-2">
        {/* View Toggle */}
        {onViewModeChange && (
          <div className="hidden sm:flex items-center border border-gray-200 dark:border-gray-700 rounded-md">
            <Button
              variant={viewMode === "list" ? "secondary" : "ghost"}
              size="icon"
              onClick={() => onViewModeChange("list")}
              className="rounded-r-none border-0"
            >
              <List className="h-4 w-4" />
            </Button>
            <Button
              variant={viewMode === "board" ? "secondary" : "ghost"}
              size="icon"
              onClick={() => onViewModeChange("board")}
              className="rounded-l-none border-0"
            >
              <Grid className="h-4 w-4" />
            </Button>
          </div>
        )}

        {/* Add Task Button */}
        {onAddTask && (
          <Button onClick={onAddTask} className="gap-2">
            <Plus className="h-4 w-4" />
            Add Task
          </Button>
        )}
      </div>
    </div>
  )
}
