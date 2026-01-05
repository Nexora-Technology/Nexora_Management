"use client"

import * as React from "react"
import { useParams, useRouter } from "next/navigation"
import { ArrowLeft, Target, Pencil, Trash2 } from "lucide-react"
import { Button } from "@/components/ui/button"
import { ObjectiveCard } from "@/components/goals/objective-card"
import { KeyResultEditor } from "@/components/goals/key-result-editor"
import { goalsApi } from "@/features/goals/api"
import { Objective, KeyResult, ObjectiveTreeNode } from "@/features/goals/types"

/**
 * Objective Detail Page
 *
 * Displays full objective details with:
 * - Objective information card
 * - Key results editor (inline editing for progress updates)
 * - Sub-objectives (if any)
 */
export default function ObjectiveDetailPage() {
  const params = useParams()
  const router = useRouter()
  const objectiveId = params.id as string

  const [isLoading, setIsLoading] = React.useState(true)
  const [error, setError] = React.useState<string | null>(null)
  const [objective, setObjective] = React.useState<Objective | null>(null)
  const [keyResults, setKeyResults] = React.useState<KeyResult[]>([])
  const [subObjectives, setSubObjectives] = React.useState<Objective[]>([])

  React.useEffect(() => {
    if (objectiveId) {
      loadObjective()
    }
  }, [objectiveId])

  const loadObjective = async () => {
    setIsLoading(true)
    setError(null)
    try {
      // In real app, you'd have a getObjectiveById endpoint
      // For now, we'll fetch from tree and find the objective
      const workspaceId = "default-workspace-id"
      const tree = await goalsApi.getObjectiveTree(workspaceId)

      const findObjective = (nodes: ObjectiveTreeNode[]): ObjectiveTreeNode | null => {
        for (const node of nodes) {
          if (node.id === objectiveId) return node
          if (node.subObjectives.length > 0) {
            const found = findObjective(node.subObjectives)
            if (found) return found
          }
        }
        return null
      }

      const found = findObjective(tree)

      if (found) {
        setObjective(found)
        setKeyResults(found.keyResults)
        setSubObjectives(found.subObjectives)
      } else {
        setError("Objective not found")
      }
    } catch (err) {
      setError("Failed to load objective details")
      console.error("Error loading objective:", err)
    } finally {
      setIsLoading(false)
    }
  }

  const handleUpdateKeyResult = async (id: string, data: Partial<KeyResult>) => {
    try {
      const updateData: {
        currentValue?: number
        targetValue?: number
        dueDate?: string
        weight?: number
      } = {}
      if (data.currentValue !== undefined) updateData.currentValue = data.currentValue
      if (data.targetValue !== undefined) updateData.targetValue = data.targetValue
      if (data.dueDate !== undefined) updateData.dueDate = data.dueDate ?? undefined
      if (data.weight !== undefined) updateData.weight = data.weight

      await goalsApi.updateKeyResult(id, updateData)
      await loadObjective()
      alert("Key result updated successfully")
    } catch (error) {
      alert("Failed to update key result")
      console.error(error)
    }
  }

  const handleDeleteKeyResult = async (id: string) => {
    if (!confirm("Are you sure you want to delete this key result?")) return

    try {
      await goalsApi.deleteKeyResult(id)
      await loadObjective()
      alert("Key result deleted")
    } catch (error) {
      alert("Failed to delete key result")
      console.error(error)
    }
  }

  const handleDeleteObjective = async () => {
    if (!confirm("Are you sure you want to delete this objective? This action cannot be undone.")) return

    try {
      await goalsApi.deleteObjective(objectiveId)
      router.push("/goals")
    } catch (error) {
      alert(error instanceof Error ? error.message : "Failed to delete objective")
      console.error(error)
    }
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500 dark:text-gray-400">Loading objective...</div>
      </div>
    )
  }

  if (error || !objective) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-error">{error || "Objective not found"}</div>
      </div>
    )
  }

  return (
    <div className="max-w-6xl mx-auto p-6">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-4">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => router.push("/goals")}
            className="gap-2"
          >
            <ArrowLeft className="w-4 h-4" />
            Back to Goals
          </Button>
          <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center">
            <Target className="w-5 h-5 text-primary" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
              {objective.title}
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              {objective.status.replace("-", " ")} • {objective.progress}% complete
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" className="gap-2">
            <Pencil className="w-4 h-4" />
            Edit
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={handleDeleteObjective}
            className="gap-2 text-error hover:text-error"
          >
            <Trash2 className="w-4 h-4" />
            Delete
          </Button>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        {/* Main Content */}
        <div className="lg:col-span-2 space-y-6">
          {/* Objective Card */}
          <ObjectiveCard
            objective={objective}
            keyResults={keyResults}
            className="cursor-default"
          />

          {/* Key Results Editor */}
          <KeyResultEditor
            keyResults={keyResults}
            onUpdate={handleUpdateKeyResult}
            onDelete={handleDeleteKeyResult}
          />

          {/* Sub-objectives */}
          {subObjectives.length > 0 && (
            <div>
              <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">
                Sub-objectives ({subObjectives.length})
              </h2>
              <div className="grid gap-4 md:grid-cols-2">
                {subObjectives.map((sub) => (
                  <ObjectiveCard
                    key={sub.id}
                    objective={sub}
                    onClick={() => router.push(`/goals/${sub.id}`)}
                  />
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Details Card */}
          <div className="bg-gray-50 dark:bg-gray-800/50 rounded-xl p-4 border border-gray-200 dark:border-gray-700">
            <h3 className="font-semibold text-gray-900 dark:text-white mb-4">
              Details
            </h3>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-gray-500 dark:text-gray-400">Weight</dt>
                <dd className="font-medium text-gray-900 dark:text-white">
                  {objective.weight}
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500 dark:text-gray-400">Status</dt>
                <dd className="font-medium capitalize text-gray-900 dark:text-white">
                  {objective.status.replace("-", " ")}
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500 dark:text-gray-400">Progress</dt>
                <dd className="font-medium text-gray-900 dark:text-white">
                  {objective.progress}%
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-gray-500 dark:text-gray-400">Key Results</dt>
                <dd className="font-medium text-gray-900 dark:text-white">
                  {keyResults.length}
                </dd>
              </div>
            </dl>
          </div>

          {/* Description */}
          {objective.description && (
            <div className="bg-gray-50 dark:bg-gray-800/50 rounded-xl p-4 border border-gray-200 dark:border-gray-700">
              <h3 className="font-semibold text-gray-900 dark:text-white mb-3">
                Description
              </h3>
              <p className="text-sm text-gray-600 dark:text-gray-400">
                {objective.description}
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
