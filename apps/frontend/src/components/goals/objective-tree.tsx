"use client"

import * as React from "react"
import { ChevronDown, ChevronRight } from "lucide-react"
import { ObjectiveCard } from "./objective-card"
import { ObjectiveTreeNode } from "@/features/goals/types"
import { cn } from "@/lib/utils"

interface ObjectiveTreeProps {
  objectives: ObjectiveTreeNode[]
  onObjectiveClick?: (objectiveId: string) => void
  className?: string
  maxDepth?: number
}

/**
 * ObjectiveTree Component
 *
 * Hierarchical tree view of objectives with expand/collapse.
 * Supports up to 3 levels of nesting (Company → Team → Individual).
 */
export function ObjectiveTree({
  objectives,
  onObjectiveClick,
  className,
  maxDepth = 3
}: ObjectiveTreeProps) {
  return (
    <div className={cn("space-y-2", className)}>
      {objectives.map((objective) => (
        <TreeNode
          key={objective.id}
          node={objective}
          onObjectiveClick={onObjectiveClick}
          depth={1}
          maxDepth={maxDepth}
        />
      ))}
    </div>
  )
}

interface TreeNodeProps {
  node: ObjectiveTreeNode
  onObjectiveClick?: (objectiveId: string) => void
  depth: number
  maxDepth: number
}

function TreeNode({ node, onObjectiveClick, depth, maxDepth }: TreeNodeProps) {
  const [isExpanded, setIsExpanded] = React.useState(depth < 2) // Auto-expand first 2 levels
  const hasChildren = node.subObjectives.length > 0

  return (
    <div className={cn(depth > 1 && "ml-6 border-l-2 border-gray-200 dark:border-gray-700 pl-4")}>
      {/* Current Node */}
      <div className="mb-2">
        {/* Expand/Collapse Button for nodes with children */}
        {hasChildren && depth < maxDepth && (
          <button
            onClick={(e) => {
              e.stopPropagation()
              setIsExpanded(!isExpanded)
            }}
            className="flex items-center gap-1 text-xs text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 mb-2 transition-colors"
          >
            {isExpanded ? (
              <ChevronDown className="w-4 h-4" />
            ) : (
              <ChevronRight className="w-4 h-4" />
            )}
            <span>
              {isExpanded ? "Collapse" : "Expand"} {node.subObjectives.length} sub-objective{node.subObjectives.length !== 1 ? "s" : ""}
            </span>
          </button>
        )}

        {/* Objective Card */}
        <ObjectiveCard
          objective={node}
          keyResults={node.keyResults}
          ownerName={node.ownerName ?? undefined}
          onClick={() => onObjectiveClick?.(node.id)}
          showSubObjectives={hasChildren && depth < maxDepth}
          subObjectiveCount={node.subObjectives.length}
        />
      </div>

      {/* Children (recursive) */}
      {hasChildren && isExpanded && depth < maxDepth && (
        <div className="space-y-2">
          {node.subObjectives.map((child) => (
            <TreeNode
              key={child.id}
              node={child}
              onObjectiveClick={onObjectiveClick}
              depth={depth + 1}
              maxDepth={maxDepth}
            />
          ))}
        </div>
      )}
    </div>
  )
}
