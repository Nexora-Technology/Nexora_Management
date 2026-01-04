"use client"

import * as React from "react"
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table"
import { Task } from "@/components/tasks"
import { TaskToolbar, TaskModal } from "@/components/tasks"
import { mockTasks } from "@/components/tasks"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Checkbox } from "@/components/ui/checkbox"
import { cn } from "@/lib/utils"
import { PRIORITY_COLORS, STATUS_LABELS } from "@/components/tasks/constants"

export default function TasksPage() {
  const [viewMode, setViewMode] = React.useState<"list" | "board">("list")
  const [selectedTasks, setSelectedTasks] = React.useState<Set<string>>(new Set())
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

  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      setSelectedTasks(new Set(mockTasks.map((t) => t.id)))
    } else {
      setSelectedTasks(new Set())
    }
  }

  const handleSelectTask = (id: string) => {
    const newSelected = new Set(selectedTasks)
    if (newSelected.has(id)) {
      newSelected.delete(id)
    } else {
      newSelected.add(id)
    }
    setSelectedTasks(newSelected)
  }

  const columns: ColumnDef<Task>[] = [
    {
      id: "select",
      header: ({ table }) => (
        <Checkbox
          checked={
            table.getIsAllPageRowsSelected() ||
            (table.getIsSomePageRowsSelected() && "indeterminate")
          }
          onCheckedChange={(checked) => handleSelectAll(checked === true)}
          aria-label="Select all"
        />
      ),
      cell: ({ row }) => (
        <Checkbox
          checked={selectedTasks.has(row.original.id)}
          onCheckedChange={() => handleSelectTask(row.original.id)}
          aria-label="Select row"
        />
      ),
      enableSorting: false,
      enableHiding: false,
    },
    {
      accessorKey: "title",
      header: "Title",
      cell: ({ row }) => (
        <button
          onClick={() => handleEditTask(row.original)}
          className="text-sm font-medium text-gray-900 dark:text-white hover:text-primary dark:hover:text-primary transition-colors text-left"
        >
          {row.getValue("title")}
        </button>
      ),
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => {
        const status = row.getValue("status") as Task["status"]
        return (
          <span className="text-xs text-gray-500 dark:text-gray-400">
            {STATUS_LABELS[status]}
          </span>
        )
      },
    },
    {
      accessorKey: "priority",
      header: "Priority",
      cell: ({ row }) => {
        const priority = row.getValue("priority") as Task["priority"]
        return (
          <div className="flex items-center gap-2">
            <div
              className={cn(
                "w-2 h-2 rounded-full",
                PRIORITY_COLORS[priority]
              )}
            />
            <span className="text-xs text-gray-500 dark:text-gray-400 capitalize">
              {priority}
            </span>
          </div>
        )
      },
    },
    {
      accessorKey: "assignee",
      header: "Assignee",
      cell: ({ row }) => {
        const assignee = row.getValue("assignee") as Task["assignee"]
        return assignee ? (
          <div className="text-sm text-gray-600 dark:text-gray-400">
            {assignee.name}
          </div>
        ) : null
      },
    },
    {
      accessorKey: "dueDate",
      header: "Due Date",
      cell: ({ row }) => {
        const dueDate = row.getValue("dueDate") as string
        return dueDate ? (
          <span className="text-sm text-gray-600 dark:text-gray-400">
            {new Date(dueDate).toLocaleDateString()}
          </span>
        ) : null
      },
    },
  ]

  const table = useReactTable({
    data: mockTasks,
    columns,
    getCoreRowModel: getCoreRowModel(),
  })

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
          onViewModeChange={setViewMode}
        />

        {viewMode === "list" && (
          <div className="rounded-md border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
            <Table>
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => {
                      return (
                        <TableHead key={header.id}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(
                                header.column.columnDef.header,
                                header.getContext()
                              )}
                        </TableHead>
                      )
                    })}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {table.getRowModel().rows?.length ? (
                  table.getRowModel().rows.map((row) => (
                    <TableRow
                      key={row.id}
                      data-state={row.getIsSelected() && "selected"}
                    >
                      {row.getVisibleCells().map((cell) => (
                        <TableCell key={cell.id}>
                          {flexRender(
                            cell.column.columnDef.cell,
                            cell.getContext()
                          )}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell
                      colSpan={columns.length}
                      className="h-24 text-center text-gray-500 dark:text-gray-400"
                    >
                      No tasks found.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>
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
