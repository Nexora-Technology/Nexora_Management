"use client";

import { useState } from "react";
import { ChevronDown, ChevronRight } from "lucide-react";
import { clsx } from "clsx";
import type { Task } from "../../tasks/types";

interface ListViewProps {
  projectId: string;
}

interface SortConfig {
  field: "title" | "status" | "priority" | "dueDate";
  direction: "asc" | "desc";
}

export function ListView({ projectId }: ListViewProps) {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [sortConfig, setSortConfig] = useState<SortConfig>({ field: "title", direction: "asc" });
  const [expandedRows, setExpandedRows] = useState<Set<string>>(new Set());

  // TODO: Fetch tasks from API
  // const { data } = useTasks(projectId);

  const toggleRow = (taskId: string) => {
    setExpandedRows((prev) => {
      const next = new Set(prev);
      if (next.has(taskId)) {
        next.delete(taskId);
      } else {
        next.add(taskId);
      }
      return next;
    });
  };

  const handleSort = (field: SortConfig["field"]) => {
    setSortConfig((prev) => ({
      field,
      direction: prev.field === field && prev.direction === "asc" ? "desc" : "asc",
    }));
  };

  const sortedTasks = [...tasks].sort((a, b) => {
    const aValue = sortConfig.field === "title" ? a.title
      : sortConfig.field === "status" ? a.statusId
      : sortConfig.field === "priority" ? a.priority
      : sortConfig.field === "dueDate" ? a.dueDate
      : "";
    const bValue = sortConfig.field === "title" ? b.title
      : sortConfig.field === "status" ? b.statusId
      : sortConfig.field === "priority" ? b.priority
      : sortConfig.field === "dueDate" ? b.dueDate
      : "";
    if (!aValue) return 1;
    if (!bValue) return -1;

    const comparison = aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
    return sortConfig.direction === "asc" ? comparison : -comparison;
  });

  return (
    <div className="list-view space-y-4">
      {/* Filters toolbar */}
      <div className="flex items-center gap-4 p-4 border rounded-lg bg-muted/30">
        <input
          type="search"
          placeholder="Search tasks..."
          className="flex-1 max-w-sm px-3 py-2 border rounded-md bg-background"
        />
        <select className="px-3 py-2 border rounded-md bg-background">
          <option value="">All Statuses</option>
        </select>
        <select className="px-3 py-2 border rounded-md bg-background">
          <option value="">All Assignees</option>
        </select>
      </div>

      {/* Tasks table */}
      <div className="border rounded-lg overflow-hidden">
        <table className="w-full">
          <thead className="bg-muted/50">
            <tr>
              <th className="w-8 px-2"></th>
              <th className="px-4 py-3 text-left">
                <button onClick={() => handleSort("title")} className="font-semibold hover:underline">
                  Title {sortConfig.field === "title" && (sortConfig.direction === "asc" ? "↑" : "↓")}
                </button>
              </th>
              <th className="px-4 py-3 text-left">
                <button onClick={() => handleSort("status")} className="font-semibold hover:underline">
                  Status {sortConfig.field === "status" && (sortConfig.direction === "asc" ? "↑" : "↓")}
                </button>
              </th>
              <th className="px-4 py-3 text-left">
                <button onClick={() => handleSort("priority")} className="font-semibold hover:underline">
                  Priority {sortConfig.field === "priority" && (sortConfig.direction === "asc" ? "↑" : "↓")}
                </button>
              </th>
              <th className="px-4 py-3 text-left">Assignee</th>
              <th className="px-4 py-3 text-left">
                <button onClick={() => handleSort("dueDate")} className="font-semibold hover:underline">
                  Due Date {sortConfig.field === "dueDate" && (sortConfig.direction === "asc" ? "↑" : "↓")}
                </button>
              </th>
            </tr>
          </thead>
          <tbody>
            {sortedTasks.map((task) => (
              <>
                <tr
                  key={task.id}
                  className={clsx(
                    "border-t hover:bg-muted/30 transition-colors",
                    expandedRows.has(task.id) && "bg-muted/10"
                  )}
                >
                  <td className="px-2">
                    <button
                      onClick={() => toggleRow(task.id)}
                      className="p-1 hover:bg-muted rounded"
                    >
                      {expandedRows.has(task.id) ? (
                        <ChevronDown className="h-4 w-4" />
                      ) : (
                        <ChevronRight className="h-4 w-4" />
                      )}
                    </button>
                  </td>
                  <td className="px-4 py-3 font-medium">{task.title}</td>
                  <td className="px-4 py-3">
                    <span className="inline-flex px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                      {task.statusId}
                    </span>
                  </td>
                  <td className="px-4 py-3 capitalize">{task.priority}</td>
                  <td className="px-4 py-3 text-muted-foreground">
                    {task.assigneeId || "Unassigned"}
                  </td>
                  <td className="px-4 py-3 text-sm">
                    {task.dueDate ? new Date(task.dueDate).toLocaleDateString() : "-"}
                  </td>
                </tr>
                {expandedRows.has(task.id) && (
                  <tr key={`${task.id}-details`} className="border-t bg-muted/10">
                    <td colSpan={6} className="px-4 py-4">
                      <div className="space-y-2">
                        <p className="text-sm text-muted-foreground">{task.description || "No description"}</p>
                        <div className="flex gap-4 text-xs text-muted-foreground">
                          <span>Created: {new Date(task.createdAt).toLocaleDateString()}</span>
                          <span>Updated: {new Date(task.updatedAt).toLocaleDateString()}</span>
                        </div>
                      </div>
                    </td>
                  </tr>
                )}
              </>
            ))}
            {sortedTasks.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-muted-foreground">
                  No tasks found. Create your first task to get started.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
