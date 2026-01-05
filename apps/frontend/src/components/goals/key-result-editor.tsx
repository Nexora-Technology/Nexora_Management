"use client"

import * as React from "react"
import { Plus, Pencil, Trash2, Save, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { cn } from "@/lib/utils"
import { KeyResult } from "@/features/goals/types"

interface KeyResultEditorProps {
  keyResults: KeyResult[]
  onUpdate?: (id: string, data: Partial<KeyResult>) => void
  onDelete?: (id: string) => void
  onCreate?: (data: Omit<KeyResult, "id" | "createdAt" | "updatedAt" | "progress">) => void
  readOnly?: boolean
  className?: string
}

/**
 * KeyResultEditor Component
 *
 * Displays and edits key results for an objective.
 * Supports inline editing for current value updates.
 */
export function KeyResultEditor({
  keyResults,
  onUpdate,
  onDelete,
  onCreate,
  readOnly = false,
  className
}: KeyResultEditorProps) {
  const [editingId, setEditingId] = React.useState<string | null>(null)
  const [editValue, setEditValue] = React.useState<number>(0)
  const [isCreating, setIsCreating] = React.useState(false)

  const handleStartEdit = (kr: KeyResult) => {
    setEditingId(kr.id)
    setEditValue(kr.currentValue)
  }

  const handleSaveEdit = (kr: KeyResult) => {
    if (editValue !== kr.currentValue) {
      onUpdate?.(kr.id, { currentValue: editValue })
    }
    setEditingId(null)
  }

  const handleCancelEdit = () => {
    setEditingId(null)
    setEditValue(0)
  }

  const progressColor = (progress: number) =>
    progress >= 80 ? "text-success" :
    progress >= 50 ? "text-warning" :
    "text-error"

  return (
    <div className={cn("space-y-3", className)}>
      {/* Header */}
      <div className="flex items-center justify-between">
        <h4 className="text-sm font-semibold text-gray-700 dark:text-gray-300">
          Key Results ({keyResults.length})
        </h4>
        {!readOnly && onCreate && (
          <Button
            size="sm"
            variant="ghost"
            onClick={() => setIsCreating(true)}
            className="h-7 text-xs"
          >
            <Plus className="w-3.5 h-3.5 mr-1" />
            Add KR
          </Button>
        )}
      </div>

      {/* Key Results List */}
      <div className="space-y-2">
        {keyResults.map((kr) => (
          <div
            key={kr.id}
            className="bg-gray-50 dark:bg-gray-800/50 rounded-lg p-3 border border-gray-200 dark:border-gray-700"
          >
            <div className="flex items-start justify-between gap-2">
              {/* Title & Description */}
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-gray-900 dark:text-white">
                  {kr.title}
                </p>
                <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                  {kr.metricType} • Target: {kr.targetValue} {kr.unit}
                </p>
              </div>

              {/* Actions */}
              {!readOnly && (
                <div className="flex items-center gap-1">
                  {editingId === kr.id ? (
                    <>
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => handleSaveEdit(kr)}
                        className="h-7 w-7 p-0 text-success"
                      >
                        <Save className="w-3.5 h-3.5" />
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={handleCancelEdit}
                        className="h-7 w-7 p-0 text-gray-500"
                      >
                        <X className="w-3.5 h-3.5" />
                      </Button>
                    </>
                  ) : (
                    <>
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => handleStartEdit(kr)}
                        className="h-7 w-7 p-0"
                      >
                        <Pencil className="w-3.5 h-3.5" />
                      </Button>
                      {onDelete && (
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => onDelete(kr.id)}
                          className="h-7 w-7 p-0 text-error hover:text-error"
                        >
                          <Trash2 className="w-3.5 h-3.5" />
                        </Button>
                      )}
                    </>
                  )}
                </div>
              )}
            </div>

            {/* Progress Section */}
            <div className="mt-3">
              {/* Edit Mode: Input field */}
              {editingId === kr.id ? (
                <div className="flex items-center gap-2">
                  <Input
                    type="number"
                    value={editValue}
                    onChange={(e) => setEditValue(parseFloat(e.target.value) || 0)}
                    className="h-8 text-sm"
                    step={kr.metricType === "percentage" ? 1 : 0.01}
                  />
                  <span className="text-xs text-gray-500">/ {kr.targetValue} {kr.unit}</span>
                </div>
              ) : (
                /* View Mode: Progress bar & values */
                <>
                  <div className="flex items-center justify-between text-xs mb-1">
                    <span className="text-gray-500 dark:text-gray-400">Progress</span>
                    <span className={cn("font-semibold", progressColor(kr.progress))}>
                      {kr.currentValue} / {kr.targetValue} {kr.unit} ({kr.progress}%)
                    </span>
                  </div>
                  <div className="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 overflow-hidden">
                    <div
                      className={cn(
                        "h-full transition-all duration-300",
                        kr.progress >= 80 ? "bg-success" :
                        kr.progress >= 50 ? "bg-warning" :
                        "bg-error"
                      )}
                      style={{ width: `${kr.progress}%` }}
                    />
                  </div>
                </>
              )}

              {/* Due Date */}
              {kr.dueDate && editingId !== kr.id && (
                <p className={cn(
                  "text-xs mt-2",
                  new Date(kr.dueDate) < new Date() && kr.progress < 80
                    ? "text-error font-medium"
                    : "text-gray-500"
                )}>
                  Due: {new Date(kr.dueDate).toLocaleDateString()}
                  {new Date(kr.dueDate) < new Date() && kr.progress < 80 && " (Overdue)"}
                </p>
              )}
            </div>
          </div>
        ))}

        {/* Empty State */}
        {keyResults.length === 0 && (
          <div className="text-center py-6 text-sm text-gray-500 dark:text-gray-400">
            No key results yet. Add measurable outcomes to track progress.
          </div>
        )}
      </div>

      {/* Create New Key Result (simplified - in real app would use modal) */}
      {isCreating && onCreate && (
        <div className="border border-dashed border-gray-300 dark:border-gray-600 rounded-lg p-3">
          <p className="text-xs text-gray-500 mb-2">Create key result modal would open here</p>
          <Button size="sm" variant="outline" onClick={() => setIsCreating(false)}>
            Cancel
          </Button>
        </div>
      )}
    </div>
  )
}
